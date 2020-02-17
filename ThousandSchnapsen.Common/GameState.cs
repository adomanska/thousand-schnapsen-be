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
        public bool GameFinished => PlayersCards.Select((playerCards, index) => index == DealerId || playerCards.IsEmpty)
            .All(finished => finished);

        public static GameState RandomState(int dealerId)
        {
            var random = new Random();
            var shuffledDeck = Constants.Deck.OrderBy(Card => random.Next()).Select(card => card.CardId).ToArray();
            CardsSet[] playersCards = new CardsSet[Constants.PLAYERS_COUNT];
            int start = 0;
            for (int playerId = 0; playerId < Constants.PLAYERS_COUNT; playerId++)
                if (playerId != dealerId)
                {
                    playersCards[playerId] = new CardsSet(shuffledDeck.Slice(start, Constants.CARDS_PER_PLAYER_COUNT).ToArray());
                    start += Constants.CARDS_PER_PLAYER_COUNT;
                }          
            playersCards[dealerId] = new CardsSet(shuffledDeck.Slice(start, Constants.REST_CARDS_COUNT).ToArray());

            return new GameState(){
                Stock = new Stack<(int PlayerId, Card Card)>(),
                PlayersUsedCards = Enumerable.Range(0, Constants.PLAYERS_COUNT).Select(playerId => new CardsSet()).ToArray(),
                PlayersPoints = new int[Constants.PLAYERS_COUNT],
                TrumpsHistory = new List<Color>(),
                DealerId = dealerId,
                NextPlayerId = (dealerId + 1) % Constants.PLAYERS_COUNT,
                PlayersCards = playersCards
            };
        }

        public PlayerState GetPlayerState(int playerId)
        {
            var stock = Stock.ToArray();
            var playersUsedCards = PlayersUsedCards.Select(cs => cs.Clone()).ToArray();
            var playersPoints = (int[])PlayersPoints.Clone();
            var trumpsHistory = TrumpsHistory.ToArray();
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
            if (Stock.Count == 1)
            {
                UpdateTrumpsAndPoints(cardId);
                NextPlayerId = GetNextPlayerId(NextPlayerId);
            }
            else if (Stock.Count == 2)
                NextPlayerId = GetNextPlayerId(NextPlayerId);
            else
                EvaluateTurn();
        }

        public Action[] GetAvailableActions()
        {
            CardsSet availableCards;
            CardsSet playerCards = PlayersCards[NextPlayerId];
            if (Stock.Count == 0)
                availableCards = playerCards;
            else
            {
                Color stockColor = Stock.First().Card.Color;
                int baseMask = (int)Math.Pow(2, Constants.CARDS_IN_COLOR_COUNT) - 1;
                int colorMask = baseMask << (Constants.CARDS_IN_COLOR_COUNT * (int)stockColor);
                availableCards = playerCards & new CardsSet(colorMask);
                if (availableCards.IsEmpty)
                    availableCards = playerCards;
            }
            return availableCards.GetCardsIds().Select(cardId => new Action
                {
                    CardId = cardId,
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

        private void MoveCard(int cardId)
        {
            if (Stock.Count == Constants.PLAYERS_COUNT - 1)
                Stock.Clear();
            PlayersCards[NextPlayerId].RemoveCard(cardId);
            PlayersUsedCards[NextPlayerId].AddCard(cardId);
            Stock.Push((PlayerId: NextPlayerId, Card: new Card(cardId)));
        }

        private void UpdateTrumpsAndPoints(int cardId)
        {
            Card card = new Card(cardId);

            if (card.IsPartOfMarriage && PlayersCards[NextPlayerId].Contains(card.SecondMarriagePart))
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
            {
                nextPlayerId = (nextPlayerId + 1) % Constants.PLAYERS_COUNT;
            }
            return nextPlayerId;
        }
    }
}
