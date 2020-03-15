using System.Collections.Generic;
using System.Linq;
using MoreLinq.Extensions;
using ThousandSchnapsen.Common.Interfaces;

namespace ThousandSchnapsen.Common.GameTree
{
    public class Node
    {
        private readonly IGameState _gameState;
        private IEnumerable<Node> _nextNodes;

        public Node(IGameState gameState) =>
            _gameState = gameState;

            public int[] Expand()
        {
            var availableActions = _gameState.GetAvailableActions();
            if (availableActions.Length == 0 || _gameState.GameFinished)
                return _gameState.PlayersPoints;
            _nextNodes = availableActions.Select(action => new Node(_gameState.PerformAction(action)));
            return _nextNodes
                .Select(node => node.Expand())
                .MaxBy(points => points[_gameState.NextPlayerId])
                .First();
        }
    }
}