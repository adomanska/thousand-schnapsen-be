using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ThousandSchnapsen.CRM.Utils;

namespace ThousandSchnapsen.CRM.Algorithms
{
    public class CfrTrainer
    {
        private readonly int[] _players = {0, 1, 2};

        private Dictionary<(int, int, int, int), StrategyData> _nodeMap =
            new Dictionary<(int, int, int, int), StrategyData>();

        private int _nodesCount;
        private int _newInfoSetsCount;
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
                    progressBar.Report(((double)i / iterations, _totalNodes.ToString()));
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

            if (!_nodeMap.TryGetValue(infoSet.RawData, out var strategyData))
            {
                strategyData = new StrategyData(availableActions.Length);
                _nodeMap.Add(infoSet.RawData, strategyData);
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
                utils[index] = Cfr(node.GetNext(availableActions[index]), playerId, newProbabilities);
                nodeUtil += strategy[index] * utils[index];
            }

            if (node.PlayerId == playerId)
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

        public void Save(string path)
        {
            var fs = new FileStream(path, FileMode.Create);

            var formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fs, _nodeMap);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }
        }

        public void Load(string path)
        {
            var fs = new FileStream(path, FileMode.Open);
            try
            {
                var formatter = new BinaryFormatter();
                _nodeMap = (Dictionary<(int, int, int, int), StrategyData>) formatter.Deserialize(fs);
                _totalNodes = _nodeMap.Count;
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }
        }
    }
}