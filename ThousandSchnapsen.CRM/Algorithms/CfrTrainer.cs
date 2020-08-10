using System;
using System.Linq;
using ThousandSchnapsen.Common.Utils;
using ThousandSchnapsen.CRM.Utils;

namespace ThousandSchnapsen.CRM.Algorithms
{
    public class CfrTrainer
    {
        private readonly int[] _players = {0, 1, 2};

        private readonly StrategyDatabase<int, (int, int, int)> _nodeMap = new StrategyDatabase<int, (int, int, int)>();

        private int _nodesCount;
        private int _newInfoSetsCount;
        private int _existingInfoSetsCount;
        private int _totalNodes;
        private readonly Random _random = new Random();

        public void Train(int iterations)
        {
            using (var progressBar = new ProgressBar())
            {
                for (var i = 1; i < iterations + 1; i++)
                {
                    foreach (var player in _players)
                    {
                        var node = new Node();
                        Cfr(node, player, new float[] {1, 1, 1});
                    }

                    _totalNodes += _newInfoSetsCount;
                    progressBar.Report(((double) i / iterations,
                        $"{_totalNodes} ({((float) _existingInfoSetsCount / _nodesCount)})"));
                    _newInfoSetsCount = 0;
                    _nodesCount = 0;
                    _existingInfoSetsCount = 0;
                }
            }
        }

        private float Cfr(Node node, int playerId, float[] probabilities)
        {
            _nodesCount++;

            if (node.IsTerminal)
                return node.GetUtil(playerId);

            var availableActions = node.AvailableActions;

            if (availableActions.Length == 1)
                return Cfr(node.GetNext(availableActions[0]), playerId, probabilities);

            var infoSet = node.InfoSet;

            if (infoSet.IsCertain)
            {
                var (_, value) = MinMaxTrainer.Train(node.PublicGameState, infoSet.PlayersCards, playerId);
                return value;
            }

            float[] strategy;
            
            if (_nodeMap.TryGetValue(infoSet.RawData, out var strategyData))
            {
                strategy = strategyData.Strategy;
                _existingInfoSetsCount++;
            }
            else if (node.PlayerId == playerId)
            {
                strategyData = new StrategyData(availableActions.Length);
                _nodeMap.AddValue(infoSet.RawData, strategyData);
                _newInfoSetsCount++;
                strategy = strategyData.Strategy;
            }
            else
            {
                strategy = new float[availableActions.Length].Populate(_ => 1f / availableActions.Length);
            }

            float nodeUtil = 0;
            var utils = new float[availableActions.Length];

            byte[] actionsIndices;

            if (node.PlayerId == playerId)
                actionsIndices = Enumerable.Range(0, availableActions.Length).Select(i => (byte) i).ToArray();
            else
            {
                var randVal = _random.NextDouble();
                byte i = 0;
                double curVal = strategy[i];
                while (curVal < randVal)
                    curVal += strategy[++i];
                actionsIndices = new[] {i};
            }

            foreach (var index in actionsIndices)
            {
                var newProbabilities = probabilities
                    .Select((prob, id) => id == node.PlayerId ? strategy[index] * prob : prob)
                    .ToArray();
                utils[index] = Cfr(node.GetNext(availableActions[index]), playerId, newProbabilities);
                nodeUtil += strategy[index] * utils[index];
            }

            if (strategyData != null && node.PlayerId == playerId)
            {
                var opponentsProbabilities =
                    probabilities.Aggregate(1f, (acc, prob) => acc * prob) / probabilities[playerId];

                strategyData.RegretSum = strategyData.RegretSum
                    .Select((regretSum, index) => regretSum + opponentsProbabilities * (nodeUtil - utils[index]))
                    .ToArray();
                strategyData.StrategySum = strategyData.StrategySum
                    .Select((strategySum, index) => strategySum + probabilities[playerId] * strategy[index])
                    .ToArray();
            }

            return nodeUtil;
        }

        public void Save(string dataDirectory) =>
            _nodeMap.Save(dataDirectory);

    }
}