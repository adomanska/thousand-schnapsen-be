﻿using System;
using System.Linq;
using System.Collections.Generic;
using MoreLinq;

namespace ThousandSchnapsen.Common.Commons
{
    public class CardsSet
    {
        private int _code;

        public CardsSet() =>
            Code = 0;

        public CardsSet(int code) =>
            Code = code;

        public CardsSet(IEnumerable<byte> cardsIds) :
            this(cardsIds.Sum(id => (int) Math.Pow(2, id)))
        {
        }

        public CardsSet(IEnumerable<Card> cards) :
            this(cards.Select(card => card.CardId))
        {
        }

        public int Code
        {
            get => _code;
            private set
            {
                _code = value;
                Count = CountSetBits(value, Constants.CardsCount);
            }
        }

        public int Count { get; private set; }

        public bool IsEmpty => Code == 0;

        public static CardsSet operator |(CardsSet cardSetA, CardsSet cardSetB) =>
            new CardsSet(cardSetA.Code | cardSetB.Code);

        public static CardsSet operator -(CardsSet cardSetA, CardsSet cardSetB) =>
            new CardsSet(cardSetA.Code & ~cardSetB.Code);

        public static CardsSet operator &(CardsSet cardSetA, CardsSet cardSetB) =>
            new CardsSet(cardSetA.Code & cardSetB.Code);

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
            (Code & (int) Math.Pow(2, cardId)) != 0;

        public bool Contains(Card card) =>
            Contains(card.CardId);

        public CardsSet Clone() =>
            new CardsSet(Code);

        public byte[] GetCardsIds()
        {
            var cardsList = new List<byte>();
            for (byte cardId = 0; cardId < Constants.CardsCount; cardId++)
                if (Contains(cardId))
                    cardsList.Add(cardId);
            return cardsList.ToArray();
        }

        public IEnumerable<Card> GetCards() =>
            GetCardsIds().Select(cardId => new Card(cardId));

        public static CardsSet Deck() =>
            new CardsSet((int) Math.Pow(2, Constants.CardsCount) - 1);

        public static CardsSet Color(Color? color) =>
            color.HasValue
                ? new CardsSet(((int) Math.Pow(2, Constants.CardsInColorCount) - 1) <<
                               (Constants.CardsInColorCount * (int) color.Value))
                : new CardsSet();

        public IEnumerable<Color> GetTrumps() =>
            Constants.Colors.Where(color => (this & Trump(color)).Count == 2);

        public Card? GetHighestInColor(Color color)
        {
            var colorCards = (this & Color(color)).GetCards().ToArray();
            return colorCards.Any()
                ? colorCards.MaxBy(card => card.Rank).First()
                : (Card?) null;
        }

        public static CardsSet GetHigherCardsSet(Card card, Color stockColor, Color? trumpColor)
        {
            int code;
            if (card.Color == stockColor && stockColor != trumpColor)
                code = (Color(stockColor).Code & ~((1 << (card.CardId + 1)) - 1)) | Color(trumpColor).Code;
            else if (card.Color == stockColor && stockColor == trumpColor)
                code = (Color(stockColor).Code & ~((1 << (card.CardId + 1)) - 1));
            else if (card.Color == trumpColor)
                code = Color(trumpColor).Code & ~((1 << (card.CardId + 1)) - 1);
            else
                code = Color(stockColor).Code | Color(trumpColor).Code;

            return new CardsSet(code);
        }

        public override string ToString() =>
            string.Join("  ", GetCardsIds().Select(cardId => new Card(cardId).ToString()));

        public override bool Equals(object obj) =>
            Equals(obj as CardsSet);

        public override int GetHashCode() =>
            Code;

        private bool Equals(CardsSet other) =>
            Code == other.Code;

        private static int CountSetBits(int code, int length)
        {
            if (code == 0)
                return 0;
            if (length == 0)
                return 0;
            if (length == 1)
                return code;

            var rightLength = length / 2;
            var leftLength = length - rightLength;
            var rightCode = code & ((1 << rightLength) - 1);
            var leftCode = code >> rightLength;

            return CountSetBits(leftCode, leftLength) + CountSetBits(rightCode, rightLength);
        }

        private static CardsSet Trump(Color color) =>
            new CardsSet(new[] {new Card(Rank.Queen, color), new Card(Rank.King, color)});
    }
}