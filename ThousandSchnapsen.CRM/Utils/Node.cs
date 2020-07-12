using System;
using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.Common.States;

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
                _possibleCardsSets = new CardsSet[] { };
            }
        }
    }
}