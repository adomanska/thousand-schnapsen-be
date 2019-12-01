using System.Collections.Generic;
using System.Linq;
using System;

namespace ThousandSchnapsen.Common
{
    public class GameState : PublicState
    {
        const int PLAYERS_COUNT = 4;
        public GameState(int[] stock, CardsSet[] playersUsedCards, int[] playersPoints, Color[] trumpsHistory,
                         int nextPlayerId, int dealerId, CardsSet[] playersCards) :
                         base(stock, playersUsedCards, playersPoints, trumpsHistory, nextPlayerId, dealerId)
        {
            PlayersCards = playersCards;
        }
        public CardsSet[] PlayersCards { get; }

        public PlayerState GetPlayerState(int playerId)
        {
            var stock = (int[])this.Stock.Clone();
            var playersUsedCards = PlayersUsedCards.Select(cs => cs.Clone()).ToArray();
            var playersPoints = (int[])this.PlayersPoints.Clone();
            var trumpsHistory = (Color[])(this.TrumpsHistory.Clone());
            var cards = PlayersCards[playerId].Clone();

            return new PlayerState(stock, playersUsedCards, playersPoints, trumpsHistory, this.NextPlayerId, cards, playerId, DealerId);
        }

        public GameState PerformAction(Action action)
        {
            var (playerId, cardId) = action;

            if (playerId != NextPlayerId)
                throw new InvalidOperationException();

            var (playersCards, playersUsedCards, stock) = MoveCard(cardId);

            Color[] trumpsHistory;
            int[] playersPoints;
            int nextPlayerId;
            if (stock.Length == 1)
            {
                (trumpsHistory, playersPoints) = UpdateTrumpsAndPoints(cardId);
                nextPlayerId = GetNextPlayerId(NextPlayerId);
            }
            else if (stock.Length == 2)
            {
                trumpsHistory = (Color[])TrumpsHistory.Clone();
                playersPoints = (int[])PlayersPoints.Clone();
                nextPlayerId = GetNextPlayerId(NextPlayerId);
            }
            else
            {
                trumpsHistory = (Color[])TrumpsHistory.Clone();
                (playersPoints, nextPlayerId) = EvaluateTurn(stock);
            }

            return new GameState(stock, playersUsedCards, playersPoints, trumpsHistory, nextPlayerId, DealerId, playersCards);
        }

        private (CardsSet[], CardsSet[], int[]) MoveCard(int cardId)
        {
            CardsSet[] playersCards = PlayersUsedCards.Select(cs => cs.Clone()).ToArray();
            playersCards[NextPlayerId].RemoveCard(cardId);
            CardsSet[] playersUsedCards = PlayersUsedCards.Select(cs => cs.Clone()).ToArray();
            playersUsedCards[NextPlayerId].AddCard(cardId);
            int[] stock = Stock.Concat(new int[] { cardId }).ToArray();

            return (playersCards, playersUsedCards, stock);
        }

        private (Color[], int[]) UpdateTrumpsAndPoints(int cardId)
        {
            Card card = new Card(cardId);
            Color[] trumpsHistory;
            int[] playersPoints;

            if (card.IsPartOfMarriage && PlayersCards[NextPlayerId].Contains(card.SecondMarriagePart))
            {
                trumpsHistory = TrumpsHistory.Concat(new Color[] { card.Color }).ToArray();
                playersPoints = (int[])PlayersPoints.Clone();
                playersPoints[NextPlayerId] += card.Color.GetPoints();
            }
            else
            {
                trumpsHistory = (Color[])TrumpsHistory.Clone();
                playersPoints = (int[])PlayersPoints.Clone();
            }

            return (trumpsHistory, playersPoints);
        }

        private (int[], int) EvaluateTurn(int[] stock)
        {
            Color firstColor = (new Card(stock[0])).Color;
            int[] cardsEval = stock.Select(cardId => (new Card(cardId)).Evaluate(firstColor, Trump)).ToArray();
            int points = stock.Sum(cardId => (new Card(cardId)).Rank.GetPoints());
            int indexMax = stock
                .Select((value, index) => new { Value = value, Index = index })
                .Aggregate((a, b) => (a.Value > b.Value) ? a : b)
                .Index;
            int nextPlayerId = NextPlayerId;
            while (indexMax < 2)
            {
                nextPlayerId = GetPrevPlayerId(nextPlayerId);
            }

            int[] playersPoints = (int[])PlayersPoints.Clone();
            playersPoints[nextPlayerId] += points;

            return (playersPoints, nextPlayerId);
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

        private int GetPrevPlayerId(int playerId)
        {
            int prevPlayerId = (playerId - 1) % PLAYERS_COUNT;
            if (prevPlayerId == DealerId)
            {
                prevPlayerId = (prevPlayerId - 1) % PLAYERS_COUNT;
            }

            return prevPlayerId;
        }
    }
}