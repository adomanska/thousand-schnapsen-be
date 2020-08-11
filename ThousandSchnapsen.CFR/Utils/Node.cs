using System.Linq;
using MoreLinq;
using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.Common.States;

namespace ThousandSchnapsen.CFR.Utils
{
    public struct NodeParams
    {
        public GameState GameState;
        public Knowledge Knowledge;
    }

    public class Node
    {
        private readonly GameState _gameState;
        private readonly Knowledge _knowledge;

        public Node()
        {
            _gameState = new GameState(3);
            var dealerCards = _gameState.PlayersCards[_gameState.DealerId];
            var opponentsIds = Enumerable.Range(0, Constants.PlayersCount)
                .Where(playerId => playerId != PlayerId && playerId != _gameState.DealerId)
                .ToArray();

            var availableCardsToLet = (_gameState.PlayersCards[PlayerId] | dealerCards);

            var cardsToLet = availableCardsToLet.GetCardsIds()
                .Shuffle()
                .Take(2)
                .Select((cardId, index) => (opponentsIds[index], cardId))
                .ToArray();

            _gameState.Init(cardsToLet);
            _knowledge = new Knowledge(dealerCards, PlayerId, cardsToLet);
        }

        public Node(NodeParams nodeParams)
        {
            _gameState = nodeParams.GameState;
            _knowledge = nodeParams.Knowledge;
        }

        public Card[] AvailableActions =>
            _gameState.GetAvailableActions().Select(action => action.Card).ToArray();

        public bool IsTerminal => _gameState.GameFinished;

        public int PlayerId => _gameState.NextPlayerId;

        public PublicState PublicGameState => _gameState;

        public InfoSet InfoSet
        {
            get
            {
                var playerCardsSet = _gameState.PlayersCards[PlayerId];

                var opponentsIds = Enumerable.Range(0, Constants.PlayersCount)
                    .Where(playerId => playerId != PlayerId && playerId != _gameState.DealerId)
                    .ToArray();

                return _knowledge.GetInfoSet(playerCardsSet, AvailableActions, PlayerId, opponentsIds);
            }
        }

        public Node GetNext(Card card)
        {
            var action = new Action() {PlayerId = PlayerId, Card = card};
            var trump = false;
            var nextGameState = _gameState.PerformAction(action, () => trump = true);
            var updatedKnowledge = _knowledge.GetNext(
                action,
                _gameState,
                trump,
                nextGameState.StockEmpty
            );

            return new Node(new NodeParams()
            {
                GameState = nextGameState,
                Knowledge = updatedKnowledge
            });
        }

        public int GetUtil(int playerId) =>
            _gameState.PlayersPoints[playerId];
    }
}