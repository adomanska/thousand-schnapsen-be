using System;
using System.Linq;
using System.Collections.Generic;

namespace ThousandSchnapsen.Common
{
    public class CardsSet
    {
        public int Code { get; private set; }

        public CardsSet() =>
            Code = 0;

        public CardsSet(int code) =>
            Code = code;

        public CardsSet(IEnumerable<int> cardsIds):
            this(cardsIds.Sum(id => (int)Math.Pow(2, id))) { }

        public CardsSet(IEnumerable<Card> cards):
            this(cards.Select(card => card.CardId)) { }

        public bool IsEmpty => Code == 0;

        public static CardsSet operator |(CardsSet A, CardsSet B) =>
            new CardsSet(A.Code | B.Code);

        public static CardsSet operator -(CardsSet A, CardsSet B) =>
            new CardsSet(A.Code & ~B.Code);

        public static CardsSet operator &(CardsSet A, CardsSet B) =>
            new CardsSet(A.Code & B.Code);

        public void AddCard(int cardId)
        {
            if (cardId < 0 || cardId > 23)
                throw new InvalidOperationException("Invalid Card ID. Card ID should be in range [0, 23].");
            Code |= (1 << cardId);
        }

        public void AddCard(Card card) =>
            AddCard(card.CardId);

        public void RemoveCard(int cardId)
        {
            if (cardId < 0 || cardId > 23)
                throw new InvalidOperationException("Invalid Card ID. Card ID should be in range [0, 23].");
            Code &= ~(1 << cardId);
        }

        public void RemoveCard(Card card) => 
            RemoveCard(card.CardId);

        public bool Contains(int cardId) => 
            (Code & (int)Math.Pow(2, cardId)) != 0;

        public bool Contains(Card card) => 
            Contains(card.CardId);

        public CardsSet Clone() => 
            new CardsSet(Code);

        public int[] GetCardsIds()
        {
            List<int> cardsList = new List<int>();
            for (int cardId = 0; cardId < Constants.CARDS_COUNT; cardId++)
                if (Contains(cardId))
                    cardsList.Add(cardId);
            return cardsList.ToArray();
        }

        public Card[] GetCards() =>
            GetCardsIds().Select(cardId => new Card(cardId)).ToArray();

        public static CardsSet Deck() =>
            new CardsSet((int)Math.Pow(2, Constants.CARDS_COUNT) - 1);

        public static CardsSet Color(Color color) =>
            new CardsSet(((int)Math.Pow(2, Constants.CARDS_IN_COLOR_COUNT) - 1) << (Constants.CARDS_IN_COLOR_COUNT * (int)color));

        public override string ToString() =>
            String.Join("  ", GetCardsIds().Select(cardId => new Card(cardId).ToString()));
    }
}
