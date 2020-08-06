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

        public Action GetAction(PlayerState playerState, Card[] availableCards)
        {
            var random = new System.Random();
            var card = availableCards
                .ElementAt(random.Next(availableCards.Length));
            return new Action()
            {
                PlayerId = _id,
                Card = card
            };
        }
    }
}