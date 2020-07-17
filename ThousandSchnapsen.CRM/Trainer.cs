using System;
using System.Collections.Generic;
using System.Linq;
using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.CRM.Utils;

namespace ThousandSchnapsen.CRM
{
    public class Trainer
    {
        private static readonly int[] Players = {
            0, 1, 2
        };
        private Dictionary<(int, int, (int, int)), StrategyData> nodeMap = new Dictionary<(int, int, (int, int)), StrategyData>();
        private int nodesCount;
        private int newInfoSetsCount = 0;
        private Random random = new Random();

        public void Train(int iterations)
        {
            double util = 0;
            for (var i = 1; i < iterations + 1; i++)
            {
                foreach (var player in Players)
                {
                    var node = new Node(null);
                    util += Crm(node, player, new float[] {1, 1, 1});
                }

                if (i % 10 == 0)
                {
                    Console.WriteLine(i.ToString("D8") + ": " + (newInfoSetsCount * 100) / nodesCount);
                    nodesCount = 0;
                    newInfoSetsCount = 0;
                }
            }
        }

        private float Crm(Node node, int playerId, float[] probs)
        {
            nodesCount++;
            if (node.IsTerminal)
                return node.GetUtil(playerId);

            var infoSet = node.InfoSet;
            if (!nodeMap.TryGetValue(infoSet, out var strategyData))
            {
                strategyData = new StrategyData(node.AvailableActions);
                nodeMap.Add(infoSet, strategyData);
                newInfoSetsCount++;
            }

            var strategy = strategyData.Strategy;
            float nodeUtil = 0;
            var utils = new float[node.AvailableActions.Length];

            IEnumerable<byte> actionsIndices;

            if (node.PlayerId == playerId)
                actionsIndices = Enumerable.Range(0, node.AvailableActions.Length).Select(i => (byte)i);
            else
            {
                var randVal = random.NextDouble();
                byte i = 0;
                double curVal = strategy[i];
                while (curVal < randVal)
                    curVal += strategy[++i];
                actionsIndices = new [] {i};
            }

            foreach (var index in actionsIndices)
            {
                var newProbabilities = probs
                    .Select((prob, id) => id == playerId ? strategy[index] * prob : prob)
                    .ToArray();
                utils[index] = Crm(node.GetNext(new Card(node.AvailableActions[index])), playerId, newProbabilities);
                nodeUtil += strategy[index] * utils[index];
            }

            if (node.PlayerId == playerId)
            {
                float oppProbs = probs.Aggregate(1f, (acc, prob) => acc * prob) / probs[playerId];

                strategyData.RegretSum = strategyData.RegretSum
                    .Select((regretSum, index) => regretSum + oppProbs * (nodeUtil - utils[index]))
                    .ToArray();
                strategyData.StrategySum = strategyData.StrategySum
                    .Select((strategySum, index) => strategySum + probs[playerId] * strategy[index])
                    .ToArray();
            }
            return nodeUtil;
        }
    }
}