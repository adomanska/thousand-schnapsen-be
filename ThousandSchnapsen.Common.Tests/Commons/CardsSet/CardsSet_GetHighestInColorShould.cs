using System;
using ThousandSchnapsen.Common.Commons;
using Xunit;

namespace ThousandSchnapsen.Common.Tests.Commons
{
    public class CardsSet_GetHighestInColorShould
    {
        [Fact]
        public void GetHighestInColor_ColorNonEmpty_ReturnHighestCard()
        {
            var cardsSetA = new CardsSet(new[] {new Card(Rank.Jack, Color.Clubs), new Card(Rank.Ace, Color.Clubs)});
            var expected = new Card(Rank.Ace, Color.Clubs);

            var result = cardsSetA.GetHighestInColor(Color.Clubs);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetHighestInColor_ColorEmpty_ReturnHighestCard()
        {
            var cardsSetA =
                new CardsSet(new[] {new Card(Rank.Jack, Color.Diamonds), new Card(Rank.Ace, Color.Diamonds)});
            Card? expected = null;

            var result = cardsSetA.GetHighestInColor(Color.Clubs);

            Assert.Equal(expected, result);
        }
    }
}