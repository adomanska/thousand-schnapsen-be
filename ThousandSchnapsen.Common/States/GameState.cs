using System;
using System.IO;
using System.Linq;
using MoreLinq;
using ThousandSchnapsen.Common.Commons;
using Action = ThousandSchnapsen.Common.Commons.Action;

namespace ThousandSchnapsen.Common.States
{
    public class GameState : PublicState
    {
        private bool initialized = false;
        public GameState()
        {
        }

        public GameState(int dealerId)
        {
            DealerId = dealerId;
            NextPlayerId = (DealerId + 1) % Constants.PlayersCount;
            PlayersCards = new CardsSet[Constants.PlayersCount];
            var shuffledDeck = Constants.Deck.Shuffle().ToArray();
            var dealtCards = 0;
            for (var playerId = 0; playerId < Constants.PlayersCount; playerId++)
            {
                var cardToDeal = playerId == dealerId ? Constants.CardsPerDealerCount : Constants.CardsPerPlayerCount;
                PlayersCards[playerId] = new CardsSet(shuffledDeck.Slice(dealtCards, cardToDeal));
                dealtCards += cardToDeal;
            }
            PlayersPoints[DealerId] = PlayersCards[DealerId]
                .GetCards()
                .Sum(card => card.Rank.GetPoints());
        }

        public CardsSet[] PlayersCards { get; set; }

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

        public void Init((int PlayerId, byte CardId)[] cardsToLet)
        {
            if (initialized)
                return;
            
            PlayersCards[NextPlayerId] |= PlayersCards[DealerId];

            if (!cardsToLet.All(item => PlayersCards[NextPlayerId].Contains(item.CardId)))
                throw new InvalidDataException("Player cannot give cards which he doesn't own");
            
            cardsToLet.ForEach(item =>
            {
                var (playerId, cardId) = item;
                PlayersCards[playerId].AddCard(cardId);
                PlayersCards[NextPlayerId].RemoveCard(cardId);
            });
        }

        public Action[] GetAvailableActions()
        {
            var availableCards = PlayersCards[NextPlayerId];
            if (Stock.Length > 0 && Stock.Length < Constants.PlayersCount - 1)
            {
                var stockColor = Stock.First().Card.Color;
                var stockColorCards = availableCards & CardsSet.Color(stockColor);
                var stockMaxValue = Stock
                    .Select(stockItem => stockItem.Card.GetValue(stockColor, Trump))
                    .Max();
                var greaterStockColorCards = new CardsSet(
                    stockColorCards
                        .GetCards()
                        .Where(card => card.GetValue(stockColor, Trump) > stockMaxValue)
                        .ToArray()
                    );
                var greaterTrumpColorCards = new CardsSet(
                    (availableCards & CardsSet.Color(Trump))
                        .GetCards()
                        .Where(card => card.GetValue(stockColor, Trump) > stockMaxValue)
                        .ToArray()
                );

                if (!greaterStockColorCards.IsEmpty)
                    availableCards = greaterStockColorCards;
                else if (!stockColorCards.IsEmpty)
                    availableCards = stockColorCards;
                else if (!greaterTrumpColorCards.IsEmpty)
                    availableCards = greaterTrumpColorCards;
            }

            return availableCards.GetCardsIds()
                .Select(cardId => new Action()
                {
                    PlayerId = NextPlayerId,
                    Card = new Card(cardId)
                }).ToArray();
        }

        public GameState PerformAction(Action action, System.Action onTrump = null)
        {
            var availableActions = GetAvailableActions();
            if (!availableActions.Contains(action))
                throw new InvalidOperationException();

            var (stock, playersCards, playersUsedCards) = MoveCard(action);
            var playersPoints = (int[]) PlayersPoints.Clone();
            int nextPlayerId;
            Color[] trumpsHistory;

            switch (stock.Length)
            {
                case 1:
                    trumpsHistory = CheckTrump(action, playersPoints, out var isTrump);
                    if (isTrump)
                        onTrump?.Invoke();
                    nextPlayerId = GetNextPlayerId(action.PlayerId);
                    break;
                case 2:
                    nextPlayerId = GetNextPlayerId(action.PlayerId);
                    trumpsHistory = (Color[]) TrumpsHistory.Clone();
                    break;
                default:
                    nextPlayerId = EvaluateTurn(playersPoints, stock);
                    trumpsHistory = (Color[]) TrumpsHistory.Clone();
                    break;
            }

            return new GameState()
            {
                DealerId = DealerId,
                NextPlayerId = nextPlayerId,
                PlayersCards = playersCards,
                PlayersUsedCards = playersUsedCards,
                Stock = stock,
                TrumpsHistory = trumpsHistory,
                PlayersPoints = playersPoints
            };
        }

        private (StockItem[], CardsSet[], CardsSet[]) MoveCard(Action action)
        {
            var playersCards = PlayersCards
                .Select(cardsSet => cardsSet.Clone())
                .ToArray();
            var playersUsedCards = PlayersUsedCards
                .Select(cardsSet => cardsSet.Clone())
                .ToArray();
            StockItem[] stock;
            var stockItem = new StockItem(action.PlayerId, action.Card);

            if (Stock.Length >= Constants.PlayersCount - 1)
                stock = new[] {stockItem};
            else
            {
                stock = new StockItem[Stock.Length + 1];
                Array.Copy(Stock, stock, Stock.Length);
                stock[Stock.Length] = stockItem;
            }

            playersCards[action.PlayerId].RemoveCard(action.Card);
            playersUsedCards[action.PlayerId].AddCard(action.Card);

            return (stock, playersCards, playersUsedCards);
        }

        private Color[] CheckTrump(Action action, int[] playersPoints, out bool isTrump)
        {
            if (!action.Card.IsPartOfMarriage || action.Card.SecondMarriagePart.HasValue &&
                !PlayersCards[action.PlayerId].Contains(action.Card.SecondMarriagePart.Value))
            {
                isTrump = false;
                return (Color[]) TrumpsHistory.Clone();
            }
            playersPoints[NextPlayerId] += action.Card.Color.GetPoints();
            var trumpsHistory = new Color[TrumpsHistory.Length + 1];
            Array.Copy(TrumpsHistory, trumpsHistory, TrumpsHistory.Length);
            trumpsHistory[TrumpsHistory.Length] = action.Card.Color;
            isTrump = true;
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