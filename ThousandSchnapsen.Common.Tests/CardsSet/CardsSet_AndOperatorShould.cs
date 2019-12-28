using Xunit;

namespace ThousandSchnapsen.Common.Tests
{
    public class CardsSet_AndOperatorShould
    {
        [Fact]
        public void AndOperator_DisjunctiveSets_ReturnEmptySet()
        {
            CardsSet A = new CardsSet(new int[] { 1, 2, 6, 10, 22 });
            CardsSet B = new CardsSet(new int[] { 0, 8, 13, 17, 20 });

            CardsSet C = A & B;

            Assert.True(C.IsEmpty);
        }

        [Fact]
        public void AndOperator_IntersectingSets_ReturnValidSet()
        {
            CardsSet A = new CardsSet(new int[] { 1, 2, 6, 10, 22 });
            CardsSet B = new CardsSet(new int[] { 0, 2, 6, 17, 20 });
            int expectedCode = 0b000000000000000001000100;

            CardsSet C = A & B;

            Assert.Equal(C.Code, expectedCode);
        }

        [Fact]
        public void PlusOperator_OneEmptySet_ReturnEmptySet()
        {
            CardsSet A = new CardsSet(new int[] { 1, 2, 6, 10, 22 });
            CardsSet B = new CardsSet(new int[] { });

            CardsSet C = A & B;

            Assert.True(C.IsEmpty);
        }
    }
}
