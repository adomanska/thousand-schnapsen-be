using System;
using Xunit;

namespace ThousandSchnapsen.Common.Tests
{
    public class CardsSet_RemoveCardShould
    {
        [Fact]
        public void RemoveCard_CardInSet_RemoveCardFromCode()
        {
            CardsSet A = new CardsSet(new int[] { 1, 2, 6, 10, 22 });
            int cardId = 2;
            int expectedCode = 0b010000000000010001000010;

            A.RemoveCard(cardId);

            Assert.Equal(A.Code, expectedCode);
        }

        [Fact]
        public void RemoveCard_CardOutOfSet_DoNothing()
        {
            CardsSet A = new CardsSet(new int[] { 1, 2, 6, 10, 22 });
            int newCardId = 17;
            int expectedCode = 0b010000000000010001000110;

            A.RemoveCard(newCardId);

            Assert.Equal(A.Code, expectedCode);
        }

        [Fact]
        public void RemoveCard_InvalidCardId_ThrowException()
        {
            CardsSet A = new CardsSet(new int[] { 1, 2, 6, 10, 22 });
            int cardId = 25;

            Assert.Throws<InvalidOperationException>(() => A.RemoveCard(cardId));
        }
    }
}
