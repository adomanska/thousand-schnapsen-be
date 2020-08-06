using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq.Extensions;
using ThousandSchnapsen.Common.States;
using Action = ThousandSchnapsen.Common.Commons.Action;

namespace ThousandSchnapsen.Common.GameTree
{
    public class Node
    {
        private readonly GameState _gameState;
        private Dictionary<Action, Node> _nextNodes;
        private Random random = new Random();

        public Node(GameState gameState) =>
            _gameState = gameState;

        public int[] Expand(int playerId)
        {
            var availableActions = _gameState.GetAvailableActions();
            if (availableActions.Length == 0 || _gameState.GameFinished)
                return _gameState.PlayersPoints;

            if (_gameState.NextPlayerId == playerId)
            {
                _nextNodes = new Dictionary<Action, Node>(
                    availableActions
                        .Select(action => new KeyValuePair<Action, Node>(
                            action,
                            new Node(_gameState.PerformAction(action)))
                        )
                );
            }
            else
            {
                _nextNodes = new Dictionary<Action, Node>();
                var action = availableActions[random.Next(0, availableActions.Length)];
                _nextNodes.Add(action, new Node(_gameState.PerformAction(action)));
            }

            return _nextNodes
                .Select(item => item.Value.Expand(playerId))
                .MaxBy(points => points[_gameState.NextPlayerId])
                .First();
        }
    }
}