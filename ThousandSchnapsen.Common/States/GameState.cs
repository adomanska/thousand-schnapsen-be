using System;
using System.Linq;
using MoreLinq;
using ThousandSchnapsen.Common.Commons;
using Action = ThousandSchnapsen.Common.Commons.Action;

namespace ThousandSchnapsen.Common.States
{
    public class GameState : PublicState
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
            StockItem[] stock, Color[] trumpsHistory, int[] playersPoints = null)
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

        public CardsSet[] PlayersCards { get; }

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
                TrumpsHistory = (Color[]) TrumpsHistory.Clone(),
                Cards = PlayersCards[playerId].Clone(),
                PlayerId = playerId
            };
        }

        public PlayerState GetNextPlayerState() =>
            GetPlayerState(NextPlayerId);

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

        public GameState PerformAction(Action action)
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

            return new GameState(DealerId, nextPlayerId, playersCards, playersUsedCards, stock, trumpsHistory,
                playersPoints);
        }

        private (StockItem[], CardsSet[], CardsSet[]) MoveCard(int playerId, Card card)
        {
            var playersCards = PlayersCards
                .Select(cardsSet => cardsSet.Clone())
                .ToArray();
            var playersUsedCards = PlayersUsedCards
                .Select(cardsSet => cardsSet.Clone())
                .ToArray();
            StockItem[] stock;

            if (Stock.Length >= Constants.PlayersCount - 1)
                stock = new[] {new StockItem(playerId, card)};
            else
            {
                stock = new StockItem[Stock.Length + 1];
                Array.Copy(Stock, stock, Stock.Length);
                stock[Stock.Length] = new StockItem(playerId, card);
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

        private int EvaluateTurn(int[] playersPoints, StockItem[] stock)
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