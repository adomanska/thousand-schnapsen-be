using System;
using System.Linq;

namespace ThousandSchnapsen.Common
{
    public class CardsSet
    {
        private int code;
        public CardsSet(int _code)
        {
            code = _code;
        }

        public CardsSet(int[] cardsIds)
        {
            code = cardsIds.Sum(id => (int)Math.Pow(2, id));
        }

        public int Code => code;

        public bool IsEmpty => Code == 0;

        public static CardsSet operator +(CardsSet A, CardsSet B)
        {
            return new CardsSet(A.Code | B.Code);
        }

        public static CardsSet operator -(CardsSet A, CardsSet B)
        {
            return new CardsSet(A.Code & ~B.Code);
        }

        public void AddCard(int cardId)
        {
            if (cardId < 0 || cardId > 23)
                throw new InvalidOperationException("Invalid Card ID. Card ID should be in range [0, 23].");
            code |= (1 << cardId);
        }

        public void RemoveCard(int cardId)
        {
            if (cardId < 0 || cardId > 23)
                throw new InvalidOperationException("Invalid Card ID. Card ID should be in range [0, 23].");
            code &= ~(1 << cardId);
        }

        public bool Contains(int cardId)
        {
            return (code & (int)Math.Pow(2, cardId)) != 0;
        }

        public bool Contains(Card card)
        {
            return this.Contains(card.CardId);
        }

        public CardsSet Clone()
        {
            return new CardsSet(Code);
        }
    }
}
