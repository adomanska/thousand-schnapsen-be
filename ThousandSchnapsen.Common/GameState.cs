using System.Linq;
using System;
using MoreLinq;

namespace ThousandSchnapsen.Common
{
    public class GameState : PublicState
    {
        const int PLAYERS_COUNT = 4;

        public CardsSet[] PlayersCards { get; set; }
        public bool GameFinished => PlayersCards.All(playerCards => playerCards.IsEmpty);

        public PlayerState GetPlayerState(int playerId)
        {
            var stock = Stock.Select(pc => (pc.PlayerId, pc.Card.Clone())).ToArray();
            var playersUsedCards = PlayersUsedCards.Select(cs => cs.Clone()).ToArray();
            var playersPoints = (int[])this.PlayersPoints.Clone();
            var trumpsHistory = (Color[])(this.TrumpsHistory.Clone());
            var cards = PlayersCards[playerId].Clone();

            return new PlayerState()
            {
                Stock = stock,
                PlayersUsedCards = playersUsedCards,
                PlayersPoints = playersPoints,
                TrumpsHistory = trumpsHistory,
                Cards = cards,
                PlayerId = playerId
            };
        }

        public void PerformAction(Action action)
        {
            var (playerId, cardId) = action;

            if (playerId != NextPlayerId)
                throw new InvalidOperationException();

            MoveCard(cardId);
            if (Stock.Length == 1)
            {
                UpdateTrumpsAndPoints(cardId);
                NextPlayerId = GetNextPlayerId(NextPlayerId);
            }
            else if (Stock.Length == 2)
                NextPlayerId = GetNextPlayerId(NextPlayerId);
            else
                EvaluateTurn();
        }

        private void MoveCard(int cardId)
        {
            PlayersCards[NextPlayerId].RemoveCard(cardId);
            PlayersUsedCards[NextPlayerId].AddCard(cardId);
            Stock = Stock.Concat(new (int PlayerId, Card Card)[] { (PlayerId: NextPlayerId, Card: new Card(cardId)) }).ToArray();
        }

        private void UpdateTrumpsAndPoints(int cardId)
        {
            Card card = new Card(cardId);

            if (card.IsPartOfMarriage && PlayersCards[NextPlayerId].Contains(card.SecondMarriagePart))
            {
                TrumpsHistory = TrumpsHistory.Concat(new Color[] { card.Color }).ToArray();
                PlayersPoints[NextPlayerId] += card.Color.GetPoints();
            }
        }

        private void EvaluateTurn()
        {
            Color firstColor = Stock[0].Card.Color;
            int points = Stock.Sum(stockItem => stockItem.Card.Rank.GetPoints());
            (NextPlayerId, _) = Stock.MaxBy(stockItem => stockItem.Card.GetValue(firstColor, Trump)).First();
            PlayersPoints[NextPlayerId] += points;
        }

        private int GetNextPlayerId(int playerId)
        {
            int nextPlayerId = (playerId + 1) % PLAYERS_COUNT;
            if (nextPlayerId == DealerId)
            {
                nextPlayerId = (nextPlayerId + 1) % PLAYERS_COUNT;
            }
            return nextPlayerId;
        }
    }
}