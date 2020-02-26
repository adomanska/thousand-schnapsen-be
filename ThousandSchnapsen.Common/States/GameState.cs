using System;
using System.Linq;
using System.Collections.Generic;
using MoreLinq;
using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.Common.Interfaces;
using Action = ThousandSchnapsen.Common.Commons.Action;

namespace ThousandSchnapsen.Common.States
{
    public class GameState : IGameState
    {
        private readonly Stack<(int PlayerId, Card Card)> _stock = new Stack<(int PlayerId, Card Card)>();
        private readonly List<Color> _trumpsHistory = new List<Color>();

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

        public GameState(int dealerId, int nextPlayerId, CardsSet[] playersCards,
            IEnumerable<(int PlayerId, Card card)> stock, IEnumerable<Color> trumpsHistory)
        {
            DealerId = dealerId;
            NextPlayerId = nextPlayerId;
            PlayersCards = playersCards;
            _stock = new Stack<(int PlayerId, Card Card)>(stock);
            _trumpsHistory = new List<Color>(trumpsHistory);
        }

        public (int PlayerId, Card Card)[] Stock => _stock.Reverse().ToArray();
        public CardsSet[] PlayersCards { get; }

        public CardsSet[] PlayersUsedCards { get; } =
            new CardsSet[Constants.PlayersCount].Select(item => new CardsSet()).ToArray();

        public int[] PlayersPoints { get; } = new int[Constants.PlayersCount];
        public Color[] TrumpsHistory => _trumpsHistory.ToArray();
        public Color? Trump => TrumpsHistory.Length > 0 ? TrumpsHistory.Last() : (Color?) null;
        public int NextPlayerId { get; private set; }
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
            if (_stock.Count <= 0)
                return availableCards.GetCardsIds().Select(cardId => new Action(NextPlayerId, new Card(cardId)))
                    .ToArray();
            var stockColorCards = CardsSet.Color(Stock.First().Card.Color);
            if (!(availableCards & stockColorCards).IsEmpty)
                availableCards &= stockColorCards;
            return availableCards.GetCardsIds().Select(cardId => new Action(NextPlayerId, new Card(cardId))).ToArray();
        }

        public void PerformAction(Action action)
        {
            var (playerId, card) = action;

            if (playerId != NextPlayerId)
                throw new InvalidOperationException();

            MoveCard(playerId, card);
            switch (_stock.Count)
            {
                case 1:
                    CheckTrump(playerId, card);
                    NextPlayerId = GetNextPlayerId(playerId);
                    break;
                case 2:
                    NextPlayerId = GetNextPlayerId(playerId);
                    break;
                default:
                    EvaluateTurn();
                    break;
            }
        }

        private void MoveCard(int playerId, Card card)
        {
            if (_stock.Count == Constants.PlayersCount - 1)
                _stock.Clear();
            PlayersCards[playerId].RemoveCard(card);
            PlayersUsedCards[playerId].AddCard(card);
            _stock.Push((PlayerId: playerId, Card: card));
        }

        private void CheckTrump(int playerId, Card card)
        {
            if (!card.IsPartOfMarriage || card.SecondMarriagePart.HasValue &&
                !PlayersCards[playerId].Contains(card.SecondMarriagePart.Value)) return;
            _trumpsHistory.Add(card.Color);
            PlayersPoints[NextPlayerId] += card.Color.GetPoints();
        }

        private int GetNextPlayerId(int playerId)
        {
            var nextPlayerId = (playerId + 1) % Constants.PlayersCount;
            if (nextPlayerId == DealerId)
                nextPlayerId = (nextPlayerId + 1) % Constants.PlayersCount;
            return nextPlayerId;
        }

        private void EvaluateTurn()
        {
            var firstColor = Stock.First().Card.Color;
            var points = Stock.Sum(stockItem => stockItem.Card.Rank.GetPoints());
            (NextPlayerId, _) = Stock.MaxBy(stockItem => stockItem.Card.GetValue(firstColor, Trump)).First();
            PlayersPoints[NextPlayerId] += points;
        }
    }
}