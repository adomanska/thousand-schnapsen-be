using System;
using Xunit;
using ThousandSchnapsen.Common.Commons;

namespace ThousandSchnapsen.Common.Tests.Commons
{
    public class CardsSet_AddCardShould
    {
        [Fact]
        public void AddCard_CardOutOfSet_SetCardInCode()
        {
            var cardsSet = new CardsSet(new byte[] {1, 2, 6, 10, 22});
            const int cardId = 17;
            const int expected = 0b010000100000010001000110;

            cardsSet.AddCard(cardId);

            Assert.Equal(cardsSet.Code, expected);
        }

        [Fact]
        public void AddCard_CardInSet_DoNothing()
        {
            var cardsSet = new CardsSet(new byte[] {1, 2, 6, 10, 22});
            const int cardId = 6;
            const int expected = 0b010000000000010001000110;

            cardsSet.AddCard(cardId);

            Assert.Equal(cardsSet.Code, expected);
        }

        [Fact]
        public void AddCard_InvalidCardId_ThrowException()
        {
            var cardsSet = new CardsSet(new byte[] {1, 2, 6, 10, 22});
            const int cardId = 25;

            Assert.Throws<InvalidOperationException>(() => cardsSet.AddCard(cardId));
        }
    }
}