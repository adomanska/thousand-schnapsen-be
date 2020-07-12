using ThousandSchnapsen.Common.Commons;

namespace ThousandSchnapsen.CRM.Utils
{
    public struct InfoSet
    {
        public CardsSet PlayerCardsSet;
        public CardsSet[] OpponentsPossibleCardsSets;
        public CardsSet[] OpponentsCertainCardsSets;
        public Card[] Stock;
    }
}