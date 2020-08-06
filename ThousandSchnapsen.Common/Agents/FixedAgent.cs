using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.Common.Interfaces;
using ThousandSchnapsen.Common.States;

namespace ThousandSchnapsen.Common.Agents
{
    public class FixedAgent : IAgent
    {
        private readonly int _id;
        private readonly List<Color> _trumps = new List<Color>();

        public FixedAgent(int id) =>
            _id = id;

        public Action GetAction(PlayerState playerState, Card[] availableCards) =>
            new Action()
            {
                PlayerId = _id,
                Card = SelectCard(playerState, availableCards)
            };

        private Card SelectCard(PlayerState playerState, Card[] availableCards)
        {
            if (playerState.Stock.Length == 0 || playerState.Stock.Length == Constants.PlayersCount - 1)
                return SelectFirstCard(playerState);
            return SelectNextCard(playerState, availableCards);
        }

        private Card SelectFirstCard(PlayerState playerState)
        {
            if (_trumps.Count > 0 && playerState.Trump == _trumps.Last())
            {
                var usedCards = playerState.PlayersUsedCards.Aggregate((acc, cardsSet) => acc | cardsSet);
                var highestOppCard =
                    (CardsSet.Deck() - usedCards - playerState.Cards).GetHighestInColor(playerState.Trump.Value);
                var highestPlayerCard = playerState.Cards.GetHighestInColor(playerState.Trump.Value);

                if (highestPlayerCard.HasValue &&
                    (!highestOppCard.HasValue || highestPlayerCard.Value.Rank > highestOppCard.Value.Rank))
                    return highestPlayerCard.Value;
            }

            if (playerState.Cards.GetTrumps().Any())
            {
                var trump = playerState.Cards.GetTrumps().Last();
                _trumps.Add(trump);
                return new Card(Rank.Queen, trump);
            }

            var maxInColors = Constants.Colors
                .Select(color =>
                {
                    var usedCards = playerState.PlayersUsedCards.Aggregate((acc, cardsSet) => acc | cardsSet);
                    var highestOppCard = (CardsSet.Deck() - usedCards - playerState.Cards).GetHighestInColor(color);
                    var highestPlayerCard = playerState.Cards.GetHighestInColor(color);
                    if (highestPlayerCard.HasValue &&
                        (!highestOppCard.HasValue || highestPlayerCard.Value.Rank > highestOppCard.Value.Rank))
                        return (Card: highestPlayerCard.Value, (playerState.Cards & CardsSet.Color(color)).Count);
                    return (Card: (Card?) null, Count: 0);
                })
                .Where(item => item.Count > 0)
                .ToArray();

            if (maxInColors.Any())
            {
                var card = maxInColors.MaxBy(item => item.Count).First().Card;
                if (card != null)
                    return card.Value;
            }

            var random = new System.Random();
            return playerState.Cards.GetCards().ElementAt(random.Next(playerState.Cards.Count));
        }

        private Card SelectNextCard(PlayerState playerState, Card[] availableCards)
        {
            var stockColor = playerState.Stock.First().Card.Color;
            var trumps = playerState.Cards.GetTrumps();

            var noTrumpColorCards = availableCards
                .Where(card => !trumps.Contains(card.Color))
                .ToArray();
            if (noTrumpColorCards.Any())
                return noTrumpColorCards
                    .MinBy(card => card.GetValue(stockColor, playerState.Trump))
                    .First();

            var noMarriagesCards = availableCards
                .Where(card => !(trumps.Contains(card.Color) && card.IsPartOfMarriage))
                .ToArray();
            if (noMarriagesCards.Any())
                return noTrumpColorCards
                    .MinBy(card => card.GetValue(stockColor, playerState.Trump))
                    .First();

            return noTrumpColorCards
                .MinBy(card => card.GetValue(stockColor, playerState.Trump))
                .First();
        }
    }
}