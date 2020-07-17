using System;
using System.Linq;
using MoreLinq;
using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.Common.States;
using ThousandSchnapsen.Common.Utils;
using Action = System.Action;

namespace ThousandSchnapsen.CRM.Utils
{
    public class Node
    {
        private GameState _gameState;
        private CardsSet[] _possibleCardsSets;
        private CardsSet[] _certainCardsSets;
        private static readonly Random Random = new Random();

        public Node(NodeParams? nodeParams)
        {
            if (nodeParams.HasValue)
            {
                _gameState = nodeParams.Value.GameState;
                _possibleCardsSets = nodeParams.Value.PossibleCardsSets;
                _certainCardsSets = nodeParams.Value.CertainCardsSet;
            }
            else
            {
                _gameState = new GameState(3);
                _possibleCardsSets = new CardsSet[Constants.PlayersCount].Populate(CardsSet.Deck);
                _certainCardsSets = new CardsSet[Constants.PlayersCount].Populate(() => new CardsSet());
                
                var dealerCards = _gameState.PlayersCards[_gameState.DealerId];
                var opponentsIds = Enumerable.Range(0, Constants.PlayersCount)
                    .Where(playerId => playerId != PlayerId && playerId != _gameState.DealerId)
                    .ToArray();
                var cardsToLet =
                    (_gameState.PlayersCards[PlayerId] | dealerCards)
                    .GetCardsIds()
                    .OrderBy(cardId => Random.Next())
                    .Take(2)
                    .Select((cardId, index) => (opponentsIds[index], cardId))
                    .ToArray();
                
                 _gameState.Init(cardsToLet);
                 _possibleCardsSets[PlayerId] |= dealerCards;
                 _certainCardsSets[PlayerId] |= dealerCards;
                 cardsToLet.ForEach(item => {
                    _possibleCardsSets[PlayerId].RemoveCard(item.cardId);
                    _certainCardsSets[PlayerId].RemoveCard(item.cardId);
                 });
                 
                 cardsToLet.ForEach(item =>
                 {
                     var (playerId, cardId) = item;
                     _possibleCardsSets[playerId] -= dealerCards;
                     _possibleCardsSets[playerId].AddCard(cardId);
                     _certainCardsSets[playerId].AddCard(cardId);
                 });
            }
        }

        public byte[] AvailableActions =>
            _gameState.GetAvailableActions().Select(action => action.Card.CardId).ToArray();

        public bool IsTerminal => _gameState.GameFinished;

        public int PlayerId => _gameState.NextPlayerId;

        public (int, int, (int, int)) InfoSet
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

                var certainCardsSets = new []{
                (_certainCardsSets[opponentsIds[0]] |
                 (_possibleCardsSets[opponentsIds[0]] - playerCardsSet - possibleCardsSet))
                    .Code,
                (_certainCardsSets[opponentsIds[1]] |
                 (_possibleCardsSets[opponentsIds[1]] - playerCardsSet - possibleCardsSet))
                    .Code
                }.OrderBy(code => code).ToArray();

            return (
                    availableCardsSet.Code,
                    possibleCardsSet.Code,
                    (certainCardsSets[0], certainCardsSets[1])
                );
            }
        }

        public Node GetNext(Card card)
        {
            var nextPossibleCardsSets = _possibleCardsSets.Select(cardsSet => cardsSet.Clone()).ToArray();
            var nextCertainCardsSets = _certainCardsSets.Select(cardsSet => cardsSet.Clone()).ToArray();
            Action onTrump = () =>
            {
                if (card.SecondMarriagePart != null)
                {
                    nextCertainCardsSets[PlayerId].AddCard(card.SecondMarriagePart.Value);
                    Enumerable.Range(0, Constants.PlayersCount)
                        .Where(playerId => playerId != PlayerId)
                        .ForEach(playerId => nextPossibleCardsSets[playerId].RemoveCard(card.SecondMarriagePart.Value));
                }
            };

            var nextGameState = _gameState.PerformAction(new Common.Commons.Action()
                {Card = card, PlayerId = PlayerId}, onTrump);
            nextPossibleCardsSets.ForEach(cardsSet => cardsSet.RemoveCard(card));
            nextCertainCardsSets.ForEach(cardsSet => cardsSet.RemoveCard(card));
            nextPossibleCardsSets[PlayerId] -= GetImpossibleCardsSet(card);
            
            return new Node(new NodeParams()
            {
                CertainCardsSet = nextCertainCardsSets,
                PossibleCardsSets = nextPossibleCardsSets,
                GameState = nextGameState
            });
        }

        private CardsSet GetImpossibleCardsSet(Card card)
        {
            if (_gameState.Stock.Length == 0 || _gameState.Stock.Length >= Constants.PlayersCount - 1)
                return new CardsSet();

            var stockColor = _gameState.Stock.First().Card.Color;
            var trumpColor = _gameState.Trump;
            var maxStockValue = _gameState.Stock
                .Select(stockItem => stockItem.Card.GetValue(stockColor, trumpColor))
                .Max();

            var impossibleCards = new CardsSet();

            if (card.Color == stockColor && card.GetValue(stockColor, trumpColor) < maxStockValue)
                impossibleCards |= new CardsSet(CardsSet.Color(stockColor).GetCards()
                    .Where(c => c.GetValue(stockColor, trumpColor) > maxStockValue)
                );
            else if (card.Color != stockColor)
            {
                impossibleCards |= CardsSet.Color(stockColor);
                if (card.GetValue(stockColor, trumpColor) < maxStockValue && trumpColor.HasValue)
                    impossibleCards |= new CardsSet(CardsSet.Color(trumpColor.Value).GetCards()
                        .Where(c => c.GetValue(stockColor, trumpColor) > maxStockValue)
                    );
            }

            return impossibleCards;
        }

        public int GetUtil(int playerId) =>
            _gameState.PlayersPoints[playerId];
    }
}