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
        private Dictionary<(int, int, int, (int, int)), StrategyData> nodeMap = new Dictionary<(int, int, int, (int, int)), StrategyData>();
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
                    util += Crm(node, player, new double[] {1, 1, 1});
                }

                if (i % 10 == 0)
                {
                    Console.WriteLine(i.ToString("D8") + ": " + (newInfoSetsCount * 100) / nodesCount);
                    nodesCount = 0;
                    newInfoSetsCount = 0;
                }
            }
        }

        private double Crm(Node node, int playerId, double[] probs)
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
            double nodeUtil = 0;
            var utils = new double[Constants.CardsCount];

            byte[] actions;

            if (node.PlayerId == playerId)
                actions = node.AvailableActions;
            else
            {
                var randVal = random.NextDouble();
                int i = 0;
                double curVal = strategy[node.AvailableActions[i]];
                while (curVal < randVal)
                    curVal += strategy[node.AvailableActions[++i]];
                actions = new[] {node.AvailableActions[i]};
            }

            foreach (var action in actions)
            {
                var newProbabilities = probs
                    .Select((prob, id) => id == playerId ? strategy[action] * prob : prob)
                    .ToArray();
                utils[action] = Crm(node.GetNext(new Card(action)), playerId, newProbabilities);
                nodeUtil += strategy[action] * utils[action];
            }

            if (node.PlayerId == playerId)
            {
                var oppProbs = probs.Aggregate(1.0, (acc, prob) => acc * prob) / probs[playerId];
                foreach (var action in actions)
                {
                    strategyData.RegretSum[action] += oppProbs * (nodeUtil - utils[action]);
                    strategyData.StrategySum[action] += probs[playerId] * strategy[action];
                }
                strategyData.UpdateStrategy();
            }
            return nodeUtil;
        }
    }
}