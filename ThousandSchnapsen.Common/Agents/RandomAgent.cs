using System.Linq;
using MoreLinq;
using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.Common.Interfaces;
using ThousandSchnapsen.Common.States;

namespace ThousandSchnapsen.Common.Agents
{
    public class RandomAgent : IAgent
    {
        public RandomAgent(int id) =>
            PlayerId = id;

        public int PlayerId { get; }

        public void Init((int, byte)[] cardsToLet, int initializerId, PublicState gameState)
        {
        }

        public Action GetAction(PlayerState playerState, Card[] availableCards)
        {
            var random = new System.Random();
            var card = availableCards
                .ElementAt(random.Next(availableCards.Length));
            return new Action()
            {
                PlayerId = PlayerId,
                Card = card
            };
        }

        public void UpdateState(Action action, PublicState newState, bool trump)
        {
        }

        public (int, byte)[] GetCardsToLet(PlayerState playerState)
        {
            var availableCardsToLet = (playerState.Cards | playerState.DealerCards);
            var opponentsIds = Enumerable.Range(0, Constants.PlayersCount)
                .Where(playerId => playerId != playerState.PlayerId && playerId != playerState.DealerId)
                .ToArray();

            var cardsToLet = availableCardsToLet.GetCardsIds()
                .Shuffle()
                .Take(2)
                .Select((cardId, index) => (opponentsIds[index], cardId))
                .ToArray();

            return cardsToLet;
        }
    }
}