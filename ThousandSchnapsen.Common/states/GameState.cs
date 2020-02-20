using System;
using System.Linq;
using System.Collections.Generic;
using MoreLinq;

namespace ThousandSchnapsen.Common
{
    public class GameState: IGameState
    {
        Stack<(int PlayerId, Card Card)> stock = new Stack<(int PlayerId, Card Card)>();
        CardsSet[] playersCards;
        CardsSet[] playersUsedCards = new CardsSet[Constants.PLAYERS_COUNT].Select(item => new CardsSet()).ToArray();
        int[] playersPoints = new int[Constants.PLAYERS_COUNT];
        List<Color> trumpsHistory = new List<Color>();
        int nextPlayerId;
        int dealerId;

        public GameState(int _dealerId)
        {
            dealerId = _dealerId;
            nextPlayerId = (dealerId + 1) % Constants.PLAYERS_COUNT;
            playersCards = Constants.Deck
                .Shuffle()
                .Batch(Constants.CARDS_PER_PLAYER_COUNT)
                .Select(cards => new CardsSet(cards))
                .ToArrayByIndex(Utils.GetCardsSetsIndexer(DealerId));
            playersPoints[DealerId] = playersCards[DealerId]
                .GetCards()
                .Sum(card => card.Rank.GetPoints());
        }

        public (int PlayerId, Card Card)[] Stock => stock.ToArray();
        public CardsSet[] PlayersCards => playersCards;
        public CardsSet[] PlayersUsedCards => playersUsedCards;
        public int[] PlayersPoints => playersPoints;
        public Color[] TrumpsHistory => trumpsHistory.ToArray();
        public Color? Trump => TrumpsHistory.Length > 0 ? TrumpsHistory.Last() : (Color?)null;
        public int NextPlayerId => nextPlayerId;
        public int DealerId => dealerId;
        public bool GameFinished => PlayersCards
            .Select((playerCards, index) => index == DealerId || playerCards.IsEmpty)
            .All(finished => finished);

        public PlayerState GetPlayerState(int playerId)
        {
            return new PlayerState()
            {
                Stock = Stock,
                PlayersUsedCards = PlayersUsedCards.Select(cs => cs.Clone()).ToArray(),
                PlayersPoints = (int[])PlayersPoints.Clone(),
                TrumpsHistory = TrumpsHistory,
                Cards = PlayersCards[playerId].Clone(),
                PlayerId = playerId
            };
        }

        public Action[] GetAvailableActions()
        {
            CardsSet availableCards = PlayersCards[NextPlayerId];
            if (stock.Count > 0)
            {
                CardsSet stockColorCards = CardsSet.Color(Stock.First().Card.Color);
                if (!(availableCards & stockColorCards).IsEmpty)
                    availableCards &= stockColorCards;
            }
            return availableCards.GetCardsIds().Select(cardId => new Action
                {
                    Card = new Card(cardId),
                    PlayerId = nextPlayerId
                }).ToArray();
        }

        public void PerformAction(Action action)
        {
            var (playerId, card) = action;

            if (playerId != NextPlayerId)
                throw new InvalidOperationException();

            MoveCard(playerId, card);
            if (stock.Count == 1)
            {
                CheckTrump(playerId, card);
                nextPlayerId = GetNextPlayerId(playerId);
            }
            else if (stock.Count == 2)
                nextPlayerId = GetNextPlayerId(playerId);
            else
                EvaluateTurn();
        }

        private void MoveCard(int playerId, Card card)
        {
            if (stock.Count == Constants.PLAYERS_COUNT - 1)
                stock.Clear();
            playersCards[playerId].RemoveCard(card);
            playersUsedCards[playerId].AddCard(card);
            stock.Push((PlayerId: playerId, Card: card));
        }

        private void CheckTrump(int playerId, Card card)
        {
            if (card.IsPartOfMarriage && PlayersCards[playerId].Contains(card.SecondMarriagePart))
            {
                trumpsHistory.Add(card.Color);
                playersPoints[NextPlayerId] += card.Color.GetPoints();
            }
        }

        private int GetNextPlayerId(int playerId)
        {
            int nextPlayerId = (playerId + 1) % Constants.PLAYERS_COUNT;
            if (nextPlayerId == dealerId)
                nextPlayerId = (nextPlayerId + 1) % Constants.PLAYERS_COUNT;
            return nextPlayerId;
        }

        private void EvaluateTurn()
        {
            Color firstColor = Stock.First().Card.Color;
            int points = Stock.Sum(stockItem => stockItem.Card.Rank.GetPoints());
            (nextPlayerId, _) = Stock.MaxBy(stockItem => stockItem.Card.GetValue(firstColor, Trump)).First();
            PlayersPoints[NextPlayerId] += points;
        }
    }
}