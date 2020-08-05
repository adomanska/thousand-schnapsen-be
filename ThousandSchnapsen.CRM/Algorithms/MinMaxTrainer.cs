using System.Linq;
using MoreLinq;
using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.Common.States;

namespace ThousandSchnapsen.CRM.Algorithms
{
    public static class MinMaxTrainer
    {
        public static int Train(PublicState publicGameState, CardsSet[] playersCards, int playerId)
        {
            playersCards[publicGameState.DealerId] = new CardsSet();
            var gameState = new GameState()
            {
                DealerId = publicGameState.DealerId,
                NextPlayerId = publicGameState.NextPlayerId,
                PlayersCards = playersCards,
                PlayersPoints = publicGameState.PlayersPoints,
                PlayersUsedCards = publicGameState.PlayersUsedCards,
                Stock = publicGameState.Stock,
                TrumpsHistory = publicGameState.TrumpsHistory
            };

            return MinMax(gameState)[playerId];
        }

        private static int[] MinMax(GameState gameState)
        {
            if (gameState.GameFinished)
                return gameState.PlayersPoints;

            return gameState.GetAvailableActions()
                .Select(action => MinMax(gameState.PerformAction(action)))
                .MaxBy(playersPoints => playersPoints[gameState.NextPlayerId])
                .First();
        }
    }
}