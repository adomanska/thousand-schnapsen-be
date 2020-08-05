using System;
using System.Linq;
using MoreLinq;
using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.Common.States;
using ThousandSchnapsen.Common.Utils;
using Action = ThousandSchnapsen.Common.Commons.Action;

namespace ThousandSchnapsen.CRM.Utils
{
    public class Node
    {
        private readonly GameState _gameState;
        private readonly CardsSet[] _possibleCardsSets;
        private readonly CardsSet[] _certainCardsSets;
        private readonly byte[] _cardsLeft;

        public Node(NodeParams? nodeParams)
        {
            if (nodeParams.HasValue)
            {
                _gameState = nodeParams.Value.GameState;
                _possibleCardsSets = nodeParams.Value.PossibleCardsSets;
                _certainCardsSets = nodeParams.Value.CertainCardsSet;
                _cardsLeft = nodeParams.Value.CardsLeft;
            }
            else
            {
                _gameState = new GameState(3);
                _possibleCardsSets = new CardsSet[Constants.PlayersCount].Populate(_ => CardsSet.Deck());
                _certainCardsSets = new CardsSet[Constants.PlayersCount].Populate(_ => new CardsSet());
                _cardsLeft = new byte[Constants.PlayersCount].Populate(_ => (byte)Constants.CardsPerPlayerCount);

                var dealerCards = _gameState.PlayersCards[_gameState.DealerId];
                var opponentsIds = Enumerable.Range(0, Constants.PlayersCount)
                    .Where(playerId => playerId != PlayerId && playerId != _gameState.DealerId)
                    .ToArray();

                var availableCardsToLet = (_gameState.PlayersCards[PlayerId] | dealerCards);

                var cardsToLet =
                    MoreEnumerable.Shuffle(availableCardsToLet.GetCardsIds())
                    .Take(2)
                    .Select((cardId, index) => (opponentsIds[index], cardId))
                    .ToArray();

                _gameState.Init(cardsToLet);

                _possibleCardsSets[PlayerId] -= dealerCards;
                _certainCardsSets[PlayerId] |= dealerCards;
                MoreEnumerable.ForEach(cardsToLet, (item, index) =>
                {
                    var (opponentId, cardId) = item;
                    
                    _possibleCardsSets[PlayerId].RemoveCard(cardId);
                    _certainCardsSets[PlayerId].RemoveCard(cardId);
                    
                    _possibleCardsSets[opponentId] -= dealerCards;
                    _possibleCardsSets[opponentId].RemoveCard(cardId);
                    _certainCardsSets[opponentId].AddCard(cardId);

                    var secondOpponentId = opponentsIds[Math.Abs(index - 1)];
                    _possibleCardsSets[secondOpponentId].RemoveCard(cardId);
                });
            }
        }

        public byte[] AvailableActions =>
            _gameState.GetAvailableActions().Select(action => action.Card.CardId).ToArray();

        public bool IsTerminal => _gameState.GameFinished;

        public int PlayerId => _gameState.NextPlayerId;

        public (int, int, int, int) InfoSet
        {
            get
            {
                var playerCardsSet = _gameState.PlayersCards[PlayerId];
                var availableCardsSet = new CardsSet(AvailableActions);

                var opponentsIds = Enumerable.Range(0, Constants.PlayersCount)
                    .Where(playerId => playerId != PlayerId && playerId != _gameState.DealerId)
                    .ToArray();

                var possibleCardsSet = (_possibleCardsSets[opponentsIds[0]] - playerCardsSet) & (
                    _possibleCardsSets[opponentsIds[1]] - playerCardsSet);

                var certainCardsSets = new[]
                {
                    (_certainCardsSets[opponentsIds[0]] |
                     (_possibleCardsSets[opponentsIds[0]] - playerCardsSet - possibleCardsSet)),
                    (_certainCardsSets[opponentsIds[1]] |
                     (_possibleCardsSets[opponentsIds[1]] - playerCardsSet - possibleCardsSet))
                };

                if (certainCardsSets[0].Count == _cardsLeft[opponentsIds[0]])
                {
                    certainCardsSets[1] |= possibleCardsSet;
                    possibleCardsSet = new CardsSet();
                }
                else if (certainCardsSets[1].Count == _cardsLeft[opponentsIds[1]])
                {
                    certainCardsSets[0] |= possibleCardsSet;
                    possibleCardsSet = new CardsSet();
                }

                var certainCardsSetsCodes = certainCardsSets
                .OrderBy(cardsSet => cardsSet.Code)
                .Select(cardsSet => cardsSet.Code)
                .ToArray();

                var data = CodeUnification.Unify(new []
                {
                    availableCardsSet.Code,
                    possibleCardsSet.Code,
                    certainCardsSetsCodes[0],
                    certainCardsSetsCodes[1]
                });

                return (
                    data[0],
                    data[1],
                    data[2],
                    data[3]
                );
            }
        }

        public Node GetNext(Card card)
        {
            var nextPossibleCardsSets = _possibleCardsSets.Select(cardsSet => cardsSet.Clone()).ToArray();
            var nextCertainCardsSets = _certainCardsSets.Select(cardsSet => cardsSet.Clone()).ToArray();
            var nextCardsLeft = new byte[Constants.PlayersCount];
            Array.Copy(_cardsLeft, nextCardsLeft, _cardsLeft.Length);
            bool trump = false;
            nextCardsLeft[PlayerId] -= 1;

            void OnTrump()
            {
                trump = true;
                if (card.SecondMarriagePart == null) return;
                nextCertainCardsSets[PlayerId].AddCard(card.SecondMarriagePart.Value);
                nextPossibleCardsSets[PlayerId].RemoveCard(card.SecondMarriagePart.Value);
                Enumerable.Range(0, Constants.PlayersCount)
                    .Where(playerId => playerId != PlayerId)
                    .ForEach(playerId => nextPossibleCardsSets[playerId].RemoveCard(card.SecondMarriagePart.Value));
            }

            var nextGameState = _gameState.PerformAction(new Action()
                {Card = card, PlayerId = PlayerId}, OnTrump);

            if (!trump && card.IsPartOfMarriage && (_gameState.Stock.Length == 0 || _gameState.Stock.Length == 3))
                nextPossibleCardsSets[PlayerId].RemoveCard(card.SecondMarriagePart.Value);

            nextPossibleCardsSets.ForEach(cardsSet => cardsSet.RemoveCard(card));
            nextCertainCardsSets[PlayerId].RemoveCard(card);
            nextPossibleCardsSets[PlayerId] -= GetImpossibleCardsSet(card);

            return new Node(new NodeParams()
            {
                CertainCardsSet = nextCertainCardsSets,
                PossibleCardsSets = nextPossibleCardsSets,
                GameState = nextGameState,
                CardsLeft = nextCardsLeft
            });
        }

        private CardsSet GetImpossibleCardsSet(Card card)
        {
            if (_gameState.Stock.Length == 0 || _gameState.Stock.Length >= Constants.PlayersCount - 1)
                return new CardsSet();

            var stockColor = _gameState.Stock.First().Card.Color;
            var trumpColor = _gameState.Trump;
            var maxStockCard = MoreEnumerable.First(_gameState.Stock
                    .MaxBy(stockItem => stockItem.Card.GetValue(stockColor, trumpColor))).Card;
            var maxStockValue = maxStockCard.GetValue(stockColor, trumpColor);

            var stockColorCardsSet = CardsSet.Color(stockColor);
            var greaterCardsSet = CardsSet.GetHigherCardsSet(maxStockCard, stockColor, trumpColor);

            if (card.Color == stockColor && card.GetValue(stockColor, trumpColor) < maxStockValue)
                return stockColorCardsSet & greaterCardsSet;
            if (card.Color != stockColor && card.GetValue(stockColor, trumpColor) > maxStockValue)
                return stockColorCardsSet;
            if (card.Color != stockColor && card.GetValue(stockColor, trumpColor) < maxStockValue)
                return stockColorCardsSet | greaterCardsSet;
            
            return new CardsSet();
        }

        public int GetUtil(int playerId) =>
            _gameState.PlayersPoints[playerId];
    }
}