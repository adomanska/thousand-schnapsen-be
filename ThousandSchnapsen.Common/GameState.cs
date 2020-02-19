using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using MoreLinq;

namespace ThousandSchnapsen.Common
{
    public class GameState
    {
        public Stack<(int PlayerId, Card Card)> Stock { get; set; }
        public CardsSet[] PlayersUsedCards { get; set; }
        public int[] PlayersPoints { get; set; }
        public List<Color> TrumpsHistory { get; set; }
        public Color? Trump => TrumpsHistory.Count > 0 ? TrumpsHistory.Last() : (Color?)null;
        public int NextPlayerId { get; set; }
        public int DealerId { get; set; }
        public CardsSet[] PlayersCards { get; set; }
        public bool GameFinished => PlayersCards
            .Select((playerCards, index) => index == DealerId || playerCards.IsEmpty)
            .All(finished => finished);

        public GameState(int dealerId)
        {
            Stock = new Stack<(int PlayerId, Card Card)>();
            PlayersUsedCards = Enumerable.Range(0, Constants.PLAYERS_COUNT).Select(playerId => new CardsSet()).ToArray();
            PlayersPoints = new int[Constants.PLAYERS_COUNT];
            TrumpsHistory = new List<Color>();
            DealerId = dealerId;
            NextPlayerId = (dealerId + 1) % Constants.PLAYERS_COUNT;
            PlayersCards = Constants.Deck
                .Shuffle()
                .Batch(Constants.CARDS_PER_PLAYER_COUNT)
                .Select(cards => new CardsSet(cards))
                .ToArray();
        }

        public PlayerState GetPlayerState(int playerId)
        {
            return new PlayerState()
            {
                Stock = Stock.ToArray(),
                PlayersUsedCards = PlayersUsedCards.Select(cs => cs.Clone()).ToArray(),
                PlayersPoints = (int[])PlayersPoints.Clone(),
                TrumpsHistory = TrumpsHistory.ToArray(),
                Cards = PlayersCards[playerId].Clone(),
                PlayerId = playerId
            };
        }

        public void PerformAction(Action action)
        {
            var (playerId, card) = action;

            if (playerId != NextPlayerId)
                throw new InvalidOperationException();

            MoveCard(playerId, card);
            if (Stock.Count == 1)
            {
                CheckTrump(playerId, card);
                NextPlayerId = GetNextPlayerId(playerId);
            }
            else if (Stock.Count == 2)
                NextPlayerId = GetNextPlayerId(playerId);
            else
                EvaluateTurn();
        }

        public Action[] GetAvailableActions()
        {
            CardsSet availableCards = PlayersCards[NextPlayerId];
            CardsSet stockColorCards = CardsSet.Color(Stock.First().Card.Color);
            if (Stock.Count > 0 && !(availableCards & stockColorCards).IsEmpty)
                availableCards &= stockColorCards;

            return availableCards.GetCardsIds().Select(cardId => new Action
                {
                    Card = new Card(cardId),
                    PlayerId = NextPlayerId
                }).ToArray();
        }

        public override string ToString()
        {
            const int lineWidth = 42;
            var sb = new StringBuilder();
            sb.AppendLine(Utils.CreateTitle("GAME STATE", lineWidth));
            sb.AppendLine("STOCK:");
            sb.AppendLine(String.Join("\n", Stock.Select(stockItem => $"{stockItem.PlayerId + 1}: {stockItem.Card}")));
            sb.AppendLine("\nRESULTS:");
            sb.AppendLine(String.Join('|', Enumerable.Range(1, PlayersPoints.Length).Select(id => String.Format("{0,4}", id))));
            sb.AppendLine(String.Join('|', PlayersPoints.Select(id => String.Format("{0,4}", id))));
            sb.AppendLine($"\nTRUMP: {Trump}");
            sb.AppendLine("PLAYERS CARDS:");
            Func<int, string> playerSymbol = playerId => playerId == DealerId ? "(D)" : (playerId == NextPlayerId ? " ->" : "   ");
            for (int playerId = 0; playerId < Constants.PLAYERS_COUNT; playerId++)
                sb.AppendLine($"{playerSymbol(playerId)} {playerId + 1}:  {PlayersCards[playerId]}");
            return sb.ToString();
        }

        private void MoveCard(int playerId, Card card)
        {
            if (Stock.Count == Constants.PLAYERS_COUNT - 1)
                Stock.Clear();
            PlayersCards[playerId].RemoveCard(card);
            PlayersUsedCards[playerId].AddCard(card);
            Stock.Push((PlayerId: playerId, Card: card));
        }

        private void CheckTrump(int playerId, Card card)
        {
            if (card.IsPartOfMarriage && PlayersCards[playerId].Contains(card.SecondMarriagePart))
            {
                TrumpsHistory.Add(card.Color);
                PlayersPoints[NextPlayerId] += card.Color.GetPoints();
            }
        }

        private void EvaluateTurn()
        {
            Color firstColor = Stock.First().Card.Color;
            int points = Stock.Sum(stockItem => stockItem.Card.Rank.GetPoints());
            (NextPlayerId, _) = Stock.MaxBy(stockItem => stockItem.Card.GetValue(firstColor, Trump)).First();
            PlayersPoints[NextPlayerId] += points;
        }

        private int GetNextPlayerId(int playerId)
        {
            int nextPlayerId = (playerId + 1) % Constants.PLAYERS_COUNT;
            if (nextPlayerId == DealerId)
                nextPlayerId = (nextPlayerId + 1) % Constants.PLAYERS_COUNT;
            return nextPlayerId;
        }
    }
}