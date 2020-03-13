using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.Common.Interfaces;
using Action = ThousandSchnapsen.Common.Commons.Action;

namespace ThousandSchnapsen.Common.Agents
{
    public class FixedAgent : IAgent
    {
        private readonly int _id;
        private readonly List<Color> _trumps = new List<Color>();

        public FixedAgent(int id) =>
            _id = id;

        public Action GetAction(IPlayerState playerState) =>
            new Action(_id, SelectCard(playerState));

        private Card SelectCard(IPlayerState playerState)
        {
            if (playerState.Stock.Length == 0 || playerState.Stock.Length == 3)
                return SelectFirstCard(playerState);
            else
                return SelectNextCard(playerState);
        }

        private Card SelectFirstCard(IPlayerState playerState)
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

            var maxInColors = Constants.Colors.Select(color =>
            {
                var usedCards = playerState.PlayersUsedCards.Aggregate((acc, cardsSet) => acc | cardsSet);
                var highestOppCard = (CardsSet.Deck() - usedCards - playerState.Cards).GetHighestInColor(color);
                var highestPlayerCard = playerState.Cards.GetHighestInColor(color);
                if (highestPlayerCard.HasValue &&
                    (!highestOppCard.HasValue || highestPlayerCard.Value.Rank > highestOppCard.Value.Rank))
                    return (Card: highestPlayerCard.Value, (playerState.Cards & CardsSet.Color(color)).Count);
                return (Card: (Card?) null, Count: 0);
            }).Where(item => item.Count > 0);

            if (maxInColors.Any())
                return maxInColors.MaxBy(item => item.Count).First().Card.Value;

            var random = new Random();
            return playerState.Cards.GetCards().ElementAt(random.Next(playerState.Cards.Count));
        }

        private Card SelectNextCard(IPlayerState playerState)
        {
            var stockColor = playerState.Stock.First().Card.Color;
            var stockColorCards = CardsSet.Color(stockColor);
            var trumpColorCards = CardsSet.Color(playerState.Trump);
            var availableCards = playerState.Cards;
            if (!(availableCards & stockColorCards).IsEmpty)
            {
                availableCards &= (stockColorCards | trumpColorCards);
                var maxInStock = playerState.Stock
                    .Select(item => item.Card.GetValue(stockColor, playerState.Trump))
                    .Max();
                var greaterCards = availableCards.GetCards()
                    .Where(card => card.GetValue(stockColor, playerState.Trump) > maxInStock);
                if (greaterCards.Any())
                    return greaterCards.MinBy(card => card.GetValue(stockColor, playerState.Trump)).First();
            }

            return availableCards.GetCards().MinBy(card => card.GetValue(stockColor, playerState.Trump)).First();
        }
    }
}