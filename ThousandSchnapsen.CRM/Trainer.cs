using System;
using System.Collections.Generic;
using System.Linq;
using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.CRM.Utils;

namespace ThousandSchnapsen.CRM
{
    public class Trainer
    {
        private static readonly int[] Players =
        {
            0, 1, 2
        };

        private readonly Dictionary<(int, int, int, int), StrategyData> _nodeMap =
            new Dictionary<(int, int, int, int), StrategyData>();

        private int _nodesCount;
        private int _newInfoSetsCount;
        private readonly Random _random = new Random();

        public void Train(int iterations)
        {
            double util = 0;
            int totalNodes = 0;
            for (var i = 1; i < iterations + 1; i++)
            {
                foreach (var player in Players)
                {
                    var node = new Node(null);
                    util += Crm(node, player, new float[] {1, 1, 1});
                }

                if (i % 10 == 0)
                {
                    totalNodes += _newInfoSetsCount;
                    Console.WriteLine($"{i:D8}: {(_newInfoSetsCount * 100) / _nodesCount}% of new nodes -- {_newInfoSetsCount} new nodes -- {totalNodes} total nodes");
                    _nodesCount = 0;
                    _newInfoSetsCount = 0;
                }
            }
        }

        private float Crm(Node node, int playerId, float[] probabilities)
        {
            _nodesCount++;
            
            if (node.IsTerminal)
                return node.GetUtil(playerId);

            var availableActions = node.AvailableActions;

            if (availableActions.Length == 1)
                return Crm(node.GetNext(new Card(availableActions[0])), playerId, probabilities);
            
            var infoSet = node.InfoSet;

            if (infoSet.Item2 == 0)
                return 0; // TODO: Handle certain info set by Min - Max

            if (!_nodeMap.TryGetValue(infoSet, out var strategyData))
            {
                strategyData = new StrategyData(availableActions.Length);
                _nodeMap.Add(infoSet, strategyData);
                _newInfoSetsCount++;
            }

            var strategy = strategyData.Strategy;
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
                    .Select((prob, id) => id == playerId ? strategy[index] * prob : prob)
                    .ToArray();
                utils[index] = Crm(node.GetNext(new Card(availableActions[index])), playerId, newProbabilities);
                nodeUtil += strategy[index] * utils[index];
            }

            if (node.PlayerId == playerId)
            {
                var opponentsProbabilities = probabilities.Aggregate(1f, (acc, prob) => acc * prob) / probabilities[playerId];

                strategyData.RegretSum = strategyData.RegretSum
                    .Select((regretSum, index) => regretSum + opponentsProbabilities * (nodeUtil - utils[index]))
                    .ToArray();
                strategyData.StrategySum = strategyData.StrategySum
                    .Select((strategySum, index) => strategySum + probabilities[playerId] * strategy[index])
                    .ToArray();
            }

            return nodeUtil;
        }
    }
}