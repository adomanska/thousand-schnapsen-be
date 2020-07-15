using System;
using ThousandSchnapsen.Common.Commons;
using Xunit;

namespace ThousandSchnapsen.Common.Tests.Commons
{
    public class CardsSet_RemoveCardShould
    {
        [Fact]
        public void RemoveCard_CardInSet_RemoveCardFromCode()
        {
            var cardsSetA = new CardsSet(new byte[] {1, 2, 6, 10, 22});
            const int cardId = 2;
            const int expected = 0b010000000000010001000010;

            cardsSetA.RemoveCard(cardId);

            Assert.Equal(expected, cardsSetA.Code);
        }

        [Fact]
        public void RemoveCard_CardOutOfSet_DoNothing()
        {
            var cardsSetA = new CardsSet(new byte[] {1, 2, 6, 10, 22});
            const int newCardId = 17;
            const int expected = 0b010000000000010001000110;

            cardsSetA.RemoveCard(newCardId);

            Assert.Equal(expected, cardsSetA.Code);
        }

        [Fact]
        public void RemoveCard_InvalidCardId_ThrowException()
        {
            var cardsSetA = new CardsSet(new byte[] {1, 2, 6, 10, 22});
            const int cardId = 25;

            Assert.Throws<InvalidOperationException>(() => cardsSetA.RemoveCard(cardId));
        }
    }
}