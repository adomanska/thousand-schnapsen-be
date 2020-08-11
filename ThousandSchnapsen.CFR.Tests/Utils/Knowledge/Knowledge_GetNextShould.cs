using System.Linq;
using ThousandSchnapsen.CFR.Utils;
using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.Common.States;
using Xunit;

namespace ThousandSchnapsen.CFR.Tests.Utils
{
    public class Knowledge_GetNextShould
    {
        private static readonly CardsSet _dealerCards = new CardsSet(new[]
        {
            new Card(Rank.Jack, Color.Diamonds),
            new Card(Rank.Ace, Color.Hearts),
            new Card(Rank.Nine, Color.Spades),
        });
        private readonly int _initializerId = 0;
        private readonly (int, byte)[] _cardsToLet =
        {
            (1, new Card(Rank.Jack, Color.Spades).CardId),
            (2, new Card(Rank.Nine, Color.Spades).CardId)
        };
        private readonly PublicState _baseGameState = new PublicState()
        {
            DealerCards = _dealerCards,
            DealerId = 3,
            NextPlayerId = 0,
        };

        private readonly byte[] _expectedCardsLeft = {7, 8, 8, 8};

        private readonly CardsSet[] _baseExpectedPossibleCardsSet =
        {
            CardsSet.Deck() - _dealerCards - new CardsSet(new[] {new Card(Rank.Jack, Color.Spades)}),
            CardsSet.Deck() - _dealerCards - new CardsSet(new[] {new Card(Rank.Jack, Color.Spades)}),
            CardsSet.Deck() - _dealerCards - new CardsSet(new[] {new Card(Rank.Jack, Color.Spades)}),
            CardsSet.Deck()
        };
        private readonly CardsSet[]_baseExpectedCertainCardsSets =
        {
            new CardsSet(new[]
            {
                new Card(Rank.Jack, Color.Diamonds),
                new Card(Rank.Ace, Color.Hearts),
            }),
            new CardsSet(new[] {new Card(Rank.Jack, Color.Spades)}),
            new CardsSet(new[] {new Card(Rank.Nine, Color.Spades)}),
            new CardsSet(),
        };
        
        [Fact]
        public void GetNext_FirstCardGivenNoMarriage_ShouldReturnUpdatedKnowledge()
        {
            var knowledge = new Knowledge(_dealerCards, _initializerId, _cardsToLet);
            var action = new Action()
            {
                PlayerId = 0,
                Card = new Card(Rank.Nine, Color.Hearts)
            };
            var expectedPossibleCardsSets = _baseExpectedPossibleCardsSet
                .Select(cardsSet => cardsSet - new CardsSet(new[] {action.Card}))
                .ToArray();

            var nextKnowledge = knowledge.GetNext(action, _baseGameState, false, true);
            
            Assert.Equal(expectedPossibleCardsSets, nextKnowledge.PossibleCardsSets);
            Assert.Equal(_baseExpectedCertainCardsSets, nextKnowledge.CertainCardsSets);
            Assert.Equal(_expectedCardsLeft, nextKnowledge.CardsLeft);
        }
        
        [Fact]
        public void GetNext_FirstMarriageCardGivenAndNoTrump_ShouldReturnUpdatedKnowledge()
        {
            var knowledge = new Knowledge(_dealerCards, _initializerId, _cardsToLet);
            var action = new Action()
            {
                PlayerId = 0,
                Card = new Card(Rank.Queen, Color.Hearts)
            };
            var expectedPossibleCardsSets = _baseExpectedPossibleCardsSet
                .Select(cardsSet => cardsSet - new CardsSet(new[] {action.Card}))
                .ToArray();
            expectedPossibleCardsSets[0] -= new CardsSet(new []{ new Card(Rank.King, Color.Hearts),  });

            var nextKnowledge = knowledge.GetNext(action, _baseGameState, false, true);
            
            Assert.Equal(expectedPossibleCardsSets, nextKnowledge.PossibleCardsSets);
            Assert.Equal(_baseExpectedCertainCardsSets, nextKnowledge.CertainCardsSets);
            Assert.Equal(_expectedCardsLeft, nextKnowledge.CardsLeft);
        }
        
        [Fact]
        public void GetNext_FirstMarriageCardGivenAndTrump_ShouldReturnUpdatedKnowledge()
        {
            var knowledge = new Knowledge(_dealerCards, _initializerId, _cardsToLet);
            var action = new Action()
            {
                PlayerId = 0,
                Card = new Card(Rank.Queen, Color.Hearts)
            };
            var expectedPossibleCardsSets = _baseExpectedPossibleCardsSet
                .Select(cardsSet => cardsSet - new CardsSet(new[] {action.Card, new Card(Rank.King, Color.Hearts)}))
                .ToArray();
            var expectedCertainCardsSets = _baseExpectedCertainCardsSets
                .Select(cardsSet => cardsSet.Clone())
                .ToArray();
            expectedCertainCardsSets[0].AddCard(new Card(Rank.King, Color.Hearts));

            var nextKnowledge = knowledge.GetNext(action, _baseGameState, true, true);
            
            Assert.Equal(expectedPossibleCardsSets, nextKnowledge.PossibleCardsSets);
            Assert.Equal(expectedCertainCardsSets, nextKnowledge.CertainCardsSets);
            Assert.Equal(_expectedCardsLeft, nextKnowledge.CardsLeft);
        }
        
        [Fact]
        public void GetNext_SecondCardGiven_ShouldReturnUpdatedKnowledge()
        {
            var knowledge = new Knowledge(_dealerCards, _initializerId, _cardsToLet);
            knowledge.CardsLeft = new byte[] { 7, 8, 8, 8 };
            var action = new Action()
            {
                PlayerId = 1,
                Card = new Card(Rank.Queen, Color.Hearts)
            };
            var expectedPossibleCardsSets = _baseExpectedPossibleCardsSet
                .Select(cardsSet => cardsSet - new CardsSet(new[] {action.Card}))
                .ToArray();
            _baseGameState.Stock = new[] {new StockItem(0, new Card(Rank.Ten, Color.Clubs))};
            expectedPossibleCardsSets[1] -= CardsSet.Color(Color.Clubs);
            var expectedCardsLeft = new byte[] {7, 7, 8, 8};
            
            var nextKnowledge = knowledge.GetNext(action, _baseGameState, false, true);
            
            Assert.Equal(expectedPossibleCardsSets, nextKnowledge.PossibleCardsSets);
            Assert.Equal(_baseExpectedCertainCardsSets, nextKnowledge.CertainCardsSets);
            Assert.Equal(expectedCardsLeft, nextKnowledge.CardsLeft);
        }
    }
}