using ThousandSchnapsen.Common.Commons;
using Xunit;

namespace ThousandSchnapsen.Common.Tests.Commons
{
    public class Card_SecondMarriagePartShould
    {
        [Fact]
        public void SecondMarriagePart_GivenKing_ReturnQueen()
        {
            var card = new Card(Rank.King, Color.Clubs);
            var expected = new Card(Rank.Queen, Color.Clubs);

            Assert.Equal(expected, card.SecondMarriagePart);
        }

        [Fact]
        public void SecondMarriagePart_GivenQueen_ReturnKing()
        {
            var card = new Card(Rank.Queen, Color.Clubs);
            var expected = new Card(Rank.King, Color.Clubs);

            Assert.Equal(expected, card.SecondMarriagePart);
        }

        [Fact]
        public void SecondMarriagePart_GivenOtherCard_ReturnNull()
        {
            var card = new Card(Rank.Jack, Color.Clubs);
            Card? expected = null;

            Assert.Equal(expected, card.SecondMarriagePart);
        }
    }
}