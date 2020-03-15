using System;
using System.Linq;
using MoreLinq;
using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.Common.Interfaces;
using Action = ThousandSchnapsen.Common.Commons.Action;

namespace ThousandSchnapsen.Common.States
{
    public class GameState : IGameState
    {
        public GameState(int dealerId)
        {
            DealerId = dealerId;
            NextPlayerId = (DealerId + 1) % Constants.PlayersCount;
            PlayersCards = Constants.Deck
                .Shuffle()
                .Batch(Constants.CardsPerPlayerCount)
                .Select(cards => new CardsSet(cards))
                .ToArrayByIndex(Utils.GetCardsSetsIndexer(DealerId));
            PlayersPoints[DealerId] = PlayersCards[DealerId]
                .GetCards()
                .Sum(card => card.Rank.GetPoints());
        }

        public GameState(int dealerId, int nextPlayerId, CardsSet[] playersCards, CardsSet[] playersUsedCards,
            (int PlayerId, Card card)[] stock, Color[] trumpsHistory, int[] playersPoints = null)
        {
            DealerId = dealerId;
            NextPlayerId = nextPlayerId;
            PlayersCards = playersCards;
            PlayersUsedCards = playersUsedCards;
            Stock = stock;
            TrumpsHistory = trumpsHistory;
            if (playersPoints != null)
                PlayersPoints = playersPoints;
        }

        public (int PlayerId, Card Card)[] Stock { get; } = { };
        public CardsSet[] PlayersCards { get; }
        public CardsSet[] PlayersUsedCards { get; } =
            new CardsSet[Constants.PlayersCount].Select(item => new CardsSet()).ToArray();
        public int[] PlayersPoints { get; } = new int[Constants.PlayersCount];
        public Color[] TrumpsHistory { get; } = { };
        public Color? Trump => TrumpsHistory.Length > 0 ? TrumpsHistory.Last() : (Color?) null;
        public int NextPlayerId { get; }
        public int DealerId { get; }

        public bool GameFinished => PlayersCards
            .Select((playerCards, index) => index == DealerId || playerCards.IsEmpty)
            .All(finished => finished);

        public PlayerState GetPlayerState(int playerId)
        {
            return new PlayerState()
            {
                DealerId = DealerId,
                NextPlayerId = NextPlayerId,
                Stock = Stock,
                PlayersUsedCards = PlayersUsedCards.Select(cs => cs.Clone()).ToArray(),
                PlayersPoints = (int[]) PlayersPoints.Clone(),
                TrumpsHistory = TrumpsHistory,
                Cards = PlayersCards[playerId].Clone(),
                PlayerId = playerId
            };
        }

        public Action[] GetAvailableActions()
        {
            var availableCards = PlayersCards[NextPlayerId];
            if (Stock.Length <= 0)
                return availableCards.GetCardsIds().Select(cardId => new Action(NextPlayerId, new Card(cardId)))
                    .ToArray();
            var stockColorCards = CardsSet.Color(Stock.First().Card.Color);
            var trumpColorCards = CardsSet.Color(Trump);
            if (!(availableCards & stockColorCards).IsEmpty)
                availableCards &= (stockColorCards | trumpColorCards);
            return availableCards.GetCardsIds().Select(cardId => new Action(NextPlayerId, new Card(cardId))).ToArray();
        }

        public IGameState PerformAction(Action action)
        {
            var (playerId, card) = action;

            if (playerId != NextPlayerId)
                throw new InvalidOperationException();

            var (stock, playersCards, playersUsedCards) = MoveCard(playerId, card);
            var playersPoints = (int[]) PlayersPoints.Clone();
            int nextPlayerId;
            Color[] trumpsHistory;
            
            switch (stock.Length)
            {
                case 1:
                    trumpsHistory = CheckTrump(playerId, card, playersPoints);
                    nextPlayerId = GetNextPlayerId(playerId);
                    break;
                case 2:
                    nextPlayerId = GetNextPlayerId(playerId);
                    trumpsHistory = (Color[]) TrumpsHistory.Clone();
                    break;
                default:
                    nextPlayerId = EvaluateTurn(playersPoints, stock);
                    trumpsHistory = (Color[]) TrumpsHistory.Clone();
                    break;
            }

            return new GameState(DealerId, nextPlayerId, playersCards, playersUsedCards, stock, trumpsHistory, playersPoints);
        }

        private ((int PlayerId, Card Card)[], CardsSet[], CardsSet[]) MoveCard(int playerId, Card card)
        {
            var playersCards = PlayersCards
                .Select(cardsSet => cardsSet.Clone())
                .ToArray();
            var playersUsedCards = PlayersUsedCards
                .Select(cardsSet => cardsSet.Clone())
                .ToArray();
            (int, Card)[] stock;

            if (Stock.Length >= Constants.PlayersCount - 1)
                stock = new[] {(playerId, card)};
            else
            {
                stock = new (int, Card)[Stock.Length + 1];
                Array.Copy(Stock, stock, Stock.Length);
                stock[Stock.Length] = (playerId, card);
            }

            playersCards[playerId].RemoveCard(card);
            playersUsedCards[playerId].AddCard(card);

            return (stock, playersCards, playersUsedCards);
        }

        private Color[] CheckTrump(int playerId, Card card, int[] playersPoints)
        {
            if (!card.IsPartOfMarriage || card.SecondMarriagePart.HasValue &&
                !PlayersCards[playerId].Contains(card.SecondMarriagePart.Value)) 
                return (Color[]) TrumpsHistory.Clone();
            playersPoints[NextPlayerId] += card.Color.GetPoints();
            var trumpsHistory = new Color[TrumpsHistory.Length + 1];
            Array.Copy(TrumpsHistory, trumpsHistory, TrumpsHistory.Length);
            trumpsHistory[TrumpsHistory.Length] = card.Color;
            return trumpsHistory;
        }

        private int GetNextPlayerId(int playerId)
        {
            var nextPlayerId = (playerId + 1) % Constants.PlayersCount;
            if (nextPlayerId == DealerId)
                nextPlayerId = (nextPlayerId + 1) % Constants.PlayersCount;
            return nextPlayerId;
        }

        private int EvaluateTurn(int[] playersPoints, (int PlayerId, Card Card)[] stock)
        {
            var firstColor = stock.First().Card.Color;
            var points = stock.Sum(stockItem => stockItem.Card.Rank.GetPoints());
            var (nextPlayerId, _) = stock
                .MaxBy(stockItem => stockItem.Card.GetValue(firstColor, Trump))
                .First();
            playersPoints[nextPlayerId] += points;
            return nextPlayerId;
        }
    }
}