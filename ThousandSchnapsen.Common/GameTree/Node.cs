using System.Collections.Generic;
using System.Linq;
using MoreLinq.Extensions;
using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.Common.States;

namespace ThousandSchnapsen.Common.GameTree
{
    public class Node
    {
        private readonly GameState _gameState;
        private Dictionary<Action, Node> _nextNodes;

        public Node(GameState gameState) =>
            _gameState = gameState;

        public int[] Expand()
        {
            var availableActions = _gameState.GetAvailableActions();
            if (availableActions.Length == 0 || _gameState.GameFinished)
                return _gameState.PlayersPoints;
            _nextNodes = new Dictionary<Action, Node>(
                availableActions
                    .Select(action => new KeyValuePair<Action, Node>(
                        action,
                        new Node(_gameState.PerformAction(action)))
                    )
            );
            return _nextNodes
                .Select(item => item.Value.Expand())
                .MaxBy(points => points[_gameState.NextPlayerId])
                .First();
        }
    }
}