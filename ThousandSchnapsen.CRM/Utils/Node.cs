using System.Linq;
using MoreLinq;
using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.Common.States;
using Action = System.Action;

namespace ThousandSchnapsen.CRM.Utils
{
    public class Node
    {
        private GameState _gameState;
        private CardsSet[] _possibleCardsSets;
        private CardsSet[] _certainCardsSets;

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
                _possibleCardsSets = Enumerable
                    .Range(0, Constants.PlayersCount)
                    .Select(playerId => CardsSet.Deck())
                    .ToArray();
                _certainCardsSets = Enumerable
                    .Range(0, Constants.PlayersCount)
                    .Select(playerId => new CardsSet())
                    .ToArray();
            }
        }

        public byte[] AvailableActions =>
            _gameState.GetAvailableActions().Select(action => action.Card.CardId).ToArray();

        public bool IsTerminal => _gameState.GameFinished;

        public int PlayerId => _gameState.NextPlayerId;

        public (int, int, int, (int, int)) InfoSet
        {
            get
            {
                var playerCardsSet = _gameState.PlayersCards[PlayerId];
                var availableCardsSet = new CardsSet(AvailableActions);
                var possibleCardsSet = Enumerable.Range(0, Constants.PlayersCount)
                    .Where(playerId => playerId != PlayerId && playerId != _gameState.DealerId)
                    .Select(playerId => _possibleCardsSets[playerId])
                    .Aggregate((cumCardsSet, cardsSet) => cumCardsSet | cardsSet);
                var certainCardsSets = Enumerable.Range(0, Constants.PlayersCount)
                    .Where(playerId => playerId != PlayerId && playerId != _gameState.DealerId)
                    .Select(playerId => _certainCardsSets[playerId].Code)
                    .ToArray();
                
                return (
                    availableCardsSet.Code,
                    (playerCardsSet - availableCardsSet).Code,
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
                    nextCertainCardsSets[PlayerId].AddCard(card.SecondMarriagePart.Value);
                Enumerable.Range(0, Constants.PlayersCount)
                    .Where(playerId => playerId != PlayerId)
                    .ForEach(playerId => nextPossibleCardsSets[playerId].RemoveCard(card.SecondMarriagePart.Value));
            };
            
            nextPossibleCardsSets.ForEach(cardsSet => cardsSet.RemoveCard(card));
            nextCertainCardsSets.ForEach(cardsSet => cardsSet.RemoveCard(card));
            nextPossibleCardsSets[PlayerId] -= GetImpossibleCardsSet(card);
            
            return new Node(new NodeParams()
            {
                CertainCardsSet = nextCertainCardsSets,
                PossibleCardsSets = nextPossibleCardsSets,
                GameState = _gameState.PerformAction(new Common.Commons.Action(){Card = card, PlayerId = PlayerId})
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