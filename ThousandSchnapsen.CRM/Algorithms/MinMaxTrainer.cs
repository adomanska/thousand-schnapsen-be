using System;
using System.Linq;
using MoreLinq;
using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.Common.States;
using Action = ThousandSchnapsen.Common.Commons.Action;

namespace ThousandSchnapsen.CRM.Algorithms
{
    public static class MinMaxTrainer
    {
        public static (Action, int) Train(PublicState publicGameState, CardsSet[] playersCards, int playerId)
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

            var (action, playersPoints) = MinMax(gameState);
            
            if (!action.HasValue)
                throw new Exception("Given game state is final.");
            
            return (action.Value, playersPoints[playerId]);
        }

        private static (Action? Action, int[] PlayersPoints) MinMax(GameState gameState)
        {
            if (gameState.GameFinished)
                return (Action: null, gameState.PlayersPoints);

            return gameState.GetAvailableActions()
                .Select(action => (Action: action, MinMax(gameState.PerformAction(action)).PlayersPoints))
                .MaxBy(minMaxResult => minMaxResult.PlayersPoints[gameState.NextPlayerId])
                .First();
        }
    }
}