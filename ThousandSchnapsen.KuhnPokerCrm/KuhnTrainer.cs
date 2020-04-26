using System;
using System.Collections.Generic;

namespace ThousandSchnapsen.KuhnPokerCrm
{
    public class KuhnTrainer
    {
        public const int Pass = 0, Bet = 1, NumActions = 2;
        public Random random = new Random();
        public Dictionary<string, Node> nodeMap = new Dictionary<string, Node>();
        
        public void Train(int iterations) {
            int[] cards = {1, 2, 3};
            double util = 0;
            for (var i = 0; i < iterations; i++) {
                Shuffle(cards);
                util += CFR(cards, "", 1, 1);
            }
            Console.WriteLine("Average game value: " + util / iterations);
        }

        private double CFR(int[] cards, string history, double p0, double p1)
        {
            var plays = history.Length;
            var player = plays % 2;
            var opponent = 1 - player;
            if (plays > 1) {
                var terminalPass = history[plays - 1] == 'p';
                var doubleBet = history.Substring(plays - 2, 2) == "bb";
                var isPlayerCardHigher = cards[player] > cards[opponent];
                if (terminalPass)
                    if (history == "pp")
                        return isPlayerCardHigher ? 1 : -1;
                    else
                        return 1;
                if (doubleBet)
                    return isPlayerCardHigher ? 2 : -2;
            }
            var infoSet = cards[player] + history;
            if (!nodeMap.TryGetValue(infoSet, out var node))
            {
                node = new Node(infoSet);
                nodeMap.Add(infoSet, node);
            }
            var strategy = node.GetStrategy(player == 0 ? p0 : p1);
            var util = new double[NumActions];
            double nodeUtil = 0;
            for (var a = 0; a < NumActions; a++) {
                var nextHistory = history + (a == 0 ? "p" : "b");
                util[a] = player == 0
                    ? - CFR(cards, nextHistory, p0 * strategy[a], p1)
                    : - CFR(cards, nextHistory, p0, p1 * strategy[a]);
                nodeUtil += strategy[a] * util[a];
            }
            for (var a = 0; a < NumActions; a++) {
                var regret = util[a] - nodeUtil;
                node.RegretSum[a] += (player == 0 ? p1 : p0) * regret;
            }
            return nodeUtil;
        }

        private void Shuffle(int[] cards)
        {
            for (var c1 = cards.Length - 1; c1 > 0; c1--) {
                var c2 = random.Next(c1 + 1);
                var tmp = cards[c1];
                cards[c1] = cards[c2];
                cards[c2] = tmp;
            }
        }
    }
}