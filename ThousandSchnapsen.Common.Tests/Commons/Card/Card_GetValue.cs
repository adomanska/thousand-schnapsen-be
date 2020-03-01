using ThousandSchnapsen.Common.Commons;
using Xunit;

namespace ThousandSchnapsen.Common.Tests.Commons
{
    public class Card_GetValue
    {
        [Fact]
        public void GetValue_ColorOfGivenTrump_ReturnValidValue()
        {
            const Color trump = Color.Clubs;
            var card = new Card(Rank.Queen, Color.Clubs);
            const int expected = 27;

            var result = card.GetValue(Color.Diamonds, trump);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetValue_ColorOfFirstCard_ReturnValidValue()
        {
            const Color firstCardColor = Color.Clubs;
            var card = new Card(Rank.Queen, Color.Clubs);
            const int expected = 15;

            var actual = card.GetValue(firstCardColor, null);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetValue_DifferentColor_ReturnValidValue()
        {
            const Color firstCardColor = Color.Clubs;
            const Color trump = Color.Hearts;
            var card = new Card(Rank.Queen, Color.Spades);
            const int expected = 3;

            var actual = card.GetValue(firstCardColor, trump);

            Assert.Equal(expected, actual);
        }
    }
}