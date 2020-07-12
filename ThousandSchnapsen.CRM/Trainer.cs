using System;
using System.Collections.Generic;
using System.Linq;
using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.Common.States;
using Action = ThousandSchnapsen.Common.Commons.Action;

namespace ThousandSchnapsen.CRM
{
    public class Trainer
    {
        private const int NumActions = 24;
        private static readonly int[] Players = {
            0, 1, 2
        };
        private Dictionary<(string, int, int), Node> nodeMap = new Dictionary<(string, int, int), Node>();
        public int nodesCount = 0;

        public void Train(int iterations)
        {
            double util = 0;
            for (var i = 0; i < iterations; i++)
            {
                foreach (var player in Players)
                {
                    var gameState = new GameState(3);
                    util += Crm(gameState, new byte[] {},  player, new double[] {1, 1, 1});
                }

                if (i % 100 == 0)
                    Console.WriteLine(i.ToString("D8") + ": " + nodesCount);
            }
        }

        private double Crm(GameState gameState, byte[] history, int playerId, double[] probs)
        {
            if (gameState.GameFinished)
            {
                return gameState.PlayersPoints[playerId];
            }
            var availableActions =
                gameState.GetAvailableActions().Select(action => (byte) action.Card.CardId).ToArray();
            var nodeKey = GetNodeKey(gameState, playerId);
            if (!nodeMap.TryGetValue(nodeKey, out var node))
            {
                node = new Node(availableActions);
                nodeMap.Add(nodeKey, node);
                nodesCount += 1;
            }

            var strategy = node.GetStrategy(probs[playerId]);
            double nodeUtil = 0;
            var a = availableActions[new Random().Next(availableActions.Length)];
            var newProbs = RecalculateReachProbabilities(strategy[a], gameState.NextPlayerId, probs);
            var nextHistory = new List<byte>(history) {a};
            var action = new Action() { PlayerId = gameState.NextPlayerId, Card = new Card(a)};
            var util = Crm(gameState.PerformAction(action), nextHistory.ToArray(), playerId, newProbs);
            nodeUtil += strategy[a] * util;

            if (gameState.NextPlayerId == playerId)
            {
                var regret = util - nodeUtil;
                node.RegretSum[a] += Players.Where(id => id != playerId).Sum(id => probs[id]) * regret;
            }
            return nodeUtil;
        }

        private double[] RecalculateReachProbabilities(double strategyValue, int playerId, double[] probs)
        {
            var newProbs = (double[])probs.Clone();
            newProbs[playerId] *= strategyValue;
            return newProbs;
        }

        private (string, int, int) GetNodeKey(GameState gameState, int playerId)
        {
            var stock = String.Join("", gameState.Stock.Select(stockItem => stockItem.Card.CardId.ToString("D2")));
            var usedCards = gameState.PlayersUsedCards.Aggregate((acc, cur) => acc | cur).Code;
            var cards = gameState.PlayersCards[playerId].Code;

            return (stock, usedCards, cards);
        }
    }
}