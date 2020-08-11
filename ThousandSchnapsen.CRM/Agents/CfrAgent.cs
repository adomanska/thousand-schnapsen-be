using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using MoreLinq;
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
        private readonly string _dataDirectory;
        private Knowledge _knowledge;
        private PublicState _gameState;
        private readonly int[] _opponentsIds;

        public CfrAgent(int playerId, int[] opponentsIds, string dataPath)
        {
            PlayerId = playerId;
            _opponentsIds = opponentsIds;
            _dataDirectory = dataPath;
        }

        public int PlayerId { get; }

        public (int, byte)[] GetCardsToLet(PlayerState playerState)
        {
            var availableCardsToLet = (playerState.Cards | playerState.DealerCards);

            var cardsToLet = availableCardsToLet.GetCardsIds()
                .Shuffle()
                .Take(2)
                .Select((cardId, index) => (_opponentsIds[index], cardId))
                .ToArray();

            // TODO: Improve cards to let selection
            return cardsToLet;
        }

        public void Init((int, byte)[] cardsToLet, int initializerId, PublicState gameState)
        {
            _knowledge = new Knowledge(gameState.DealerCards, initializerId, cardsToLet);
            _gameState = gameState;
        }

        public Action GetAction(PlayerState playerState, Card[] availableCards)
        {
            var infoSet = _knowledge.GetInfoSet(playerState.Cards, availableCards, PlayerId, _opponentsIds);
            Card card;

            if (infoSet.IsCertain)
            {
                var (action, _) = MinMaxTrainer.Train(playerState, infoSet.PlayersCards, PlayerId);
                card = action.Card;
            }
            else
            {
                var (primaryKey, secondaryKey) = infoSet.RawData;
                var dataPath = $"{_dataDirectory}/{primaryKey}.dat";

                if (LoadData(dataPath, out var data) && data.TryGetValue(secondaryKey, out var strategyData))
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
            _knowledge = _knowledge.GetNext(action, _gameState, trump, newState.StockEmpty);
            _gameState = newState;
        }

        private bool LoadData(string path, out Dictionary<(int, int, int), StrategyData> data)
        {
            if (!File.Exists(path))
            {
                data = null;
                return false;
            }

            using (var fs = new FileStream(path, FileMode.Open))
            {
                try
                {
                    var formatter = new BinaryFormatter();
                    data = (Dictionary<(int, int, int), StrategyData>) formatter.Deserialize(fs);
                    return true;
                }
                catch (SerializationException e)
                {
                    Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                    throw;
                }
            }
        }
    }
}