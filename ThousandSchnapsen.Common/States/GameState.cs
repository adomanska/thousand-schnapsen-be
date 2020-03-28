using System.Linq;
using MoreLinq;
using ThousandSchnapsen.Common.Commons;

namespace ThousandSchnapsen.Common.States
{
    public class GameState : PublicState
    {
        public GameState()
        {
        }

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

        public Action[] GetAvailableActions()
        {
            var availableCards = PlayersCards[NextPlayerId];
            if (Stock.Length > 0 && Stock.Length < Constants.PlayersCount - 1)
            {
                var stockColorCards = CardsSet.Color(Stock.First().Card.Color);
                var trumpColorCards = CardsSet.Color(Trump);
                if (!(availableCards & stockColorCards).IsEmpty)
                    availableCards &= (stockColorCards | trumpColorCards);
            }

            return availableCards.GetCardsIds()
                .Select(cardId => new Action()
                {
                    PlayerId = NextPlayerId,
                    Card = new Card(cardId)
                }).ToArray();
        }

        public GameState PerformAction(Action action)
        {
            var availableActions = GetAvailableActions();
            if (!availableActions.Contains(action))
                throw new System.InvalidOperationException();

            var (stock, playersCards, playersUsedCards) = MoveCard(action);
            var playersPoints = (int[]) PlayersPoints.Clone();
            int nextPlayerId;
            Color[] trumpsHistory;

            switch (stock.Length)
            {
                case 1:
                    trumpsHistory = CheckTrump(action, playersPoints);
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
                System.Array.Copy(Stock, stock, Stock.Length);
                stock[Stock.Length] = stockItem;
            }

            playersCards[action.PlayerId].RemoveCard(action.Card);
            playersUsedCards[action.PlayerId].AddCard(action.Card);

            return (stock, playersCards, playersUsedCards);
        }

        private Color[] CheckTrump(Action action, int[] playersPoints)
        {
            if (!action.Card.IsPartOfMarriage || action.Card.SecondMarriagePart.HasValue &&
                !PlayersCards[action.PlayerId].Contains(action.Card.SecondMarriagePart.Value))
                return (Color[]) TrumpsHistory.Clone();
            playersPoints[NextPlayerId] += action.Card.Color.GetPoints();
            var trumpsHistory = new Color[TrumpsHistory.Length + 1];
            System.Array.Copy(TrumpsHistory, trumpsHistory, TrumpsHistory.Length);
            trumpsHistory[TrumpsHistory.Length] = action.Card.Color;
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