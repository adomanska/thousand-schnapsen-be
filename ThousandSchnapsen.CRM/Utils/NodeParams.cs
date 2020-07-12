using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.Common.States;

namespace ThousandSchnapsen.CRM.Utils
{
    public struct NodeParams
    {
        public GameState GameState;
        public CardsSet[] PossibleCardsSets;
        public CardsSet[] CertainCardsSet;
    }
}