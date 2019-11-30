using Xunit;

namespace ThousandSchnapsen.Common.Tests
{
    public class CardsSet_MinusOperatorShould
    {
        [Fact]
        public void MinusOperator_DisjunctiveSets_ReturnValidSet()
        {
            CardsSet A = new CardsSet(new int[]{1, 2, 6, 10, 22});
            CardsSet B = new CardsSet(new int[]{0, 8, 13, 17, 20});
            int expectedCode  = 0b010000000000010001000110;

            var C = A - B;

            Assert.Equal(C.Code, expectedCode);
        }

        [Fact]
        public void MinusOperator_IntersectingSets_ReturnValidSet()
        {
            CardsSet A = new CardsSet(new int[]{1, 2, 6, 10, 22});
            CardsSet B = new CardsSet(new int[]{0, 2, 6, 17, 20});
            int expectedCode  = 0b010000000000010000000010;

            var C = A - B;

            Assert.Equal(C.Code, expectedCode);
        }
    }
}
