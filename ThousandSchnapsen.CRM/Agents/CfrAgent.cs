using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.Common.Interfaces;
using ThousandSchnapsen.Common.States;
using ThousandSchnapsen.CRM.Algorithms;
using ThousandSchnapsen.CRM.Utils;
using Action = ThousandSchnapsen.Common.Commons.Action;

namespace ThousandSchnapsen.CRM.Agents
{
    public class CfrAgent : IAgent
    {
        private Dictionary<(int, int, int, int), StrategyData> _nodeMap;
        private Knowledge _knowledge;
        private PublicState _gameState;
        private int[] _opponentsIds;

        public CfrAgent(int playerId, int[] opponentsIds, string dataPath, Knowledge initialKnowledge)
        {
            PlayerId = playerId;
            _opponentsIds = opponentsIds;
            _knowledge = initialKnowledge;
            LoadData(dataPath);
        }

        public int PlayerId { get; }

        public Action GetAction(PlayerState playerState, Card[] availableCards)
        {
            var infoSet = _knowledge.GetInfoSet(playerState.Cards, availableCards, PlayerId, _opponentsIds);
            Card card;

            if (infoSet.IsCertain)
            {
                var (action, _) = MinMaxTrainer.Train(playerState, infoSet.PlayersCards, PlayerId);
                card = action.Card;
            }
            else if (_nodeMap.TryGetValue(infoSet.RawData, out var strategyData))
            {
                var strategy = strategyData.AverageStrategy;
                var randVal = new Random().NextDouble();
                byte actionIndex = 0;
                double curVal = strategy[actionIndex];
                while (curVal < randVal)
                    curVal += strategy[++actionIndex];
                card = availableCards[actionIndex];
            }
            else
            {
                card = availableCards[new Random().Next(availableCards.Length)];
            }

            return new Action()
            {
                PlayerId = PlayerId,
                Card = card
            };
        }

        public void UpdateState(Action action, PublicState newState, bool trump)
        {
            _knowledge = _knowledge.GetNext(action, _gameState, trump);
            _gameState = newState;
        }

        private void LoadData(string path)
        {
            var fs = new FileStream(path, FileMode.Open);
            try
            {
                var formatter = new BinaryFormatter();
                _nodeMap = (Dictionary<(int, int, int, int), StrategyData>) formatter.Deserialize(fs);
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