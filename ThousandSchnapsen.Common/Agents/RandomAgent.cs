using System.Linq;
using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.Common.Interfaces;
using ThousandSchnapsen.Common.States;

namespace ThousandSchnapsen.Common.Agents
{
    public class RandomAgent : IAgent
    {
        private readonly int _id;

        public RandomAgent(int id) =>
            _id = id;

        public Action GetAction(PlayerState playerState)
        {
            var availableCards = playerState.Cards;
            if (playerState.Stock.Length > 0)
            {
                var stockColorCards = CardsSet.Color(playerState.Stock.First().Card.Color);
                var trumpColorCards = CardsSet.Color(playerState.Trump);
                if (!(availableCards & stockColorCards).IsEmpty)
                    availableCards &= (stockColorCards | trumpColorCards);
            }

            var random = new System.Random();
            var card = availableCards
                .GetCards()
                .ElementAt(random.Next(availableCards.Count));
            return new Action()
            {
                PlayerId = _id,
                Card = card
            };
        }
    }
}