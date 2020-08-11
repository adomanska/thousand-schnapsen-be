using System;
using System.Linq;
using MoreLinq;
using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.Common.Interfaces;
using ThousandSchnapsen.Common.States;

namespace ThousandSchnapsen.CFR.Utils
{
    public class Simulation
    {
        private readonly IAgent[] _agents;
        private readonly int _dealerId;

        public Simulation(IAgent[] agents, int dealerId)
        {
            if (agents.Length != Constants.PlayersCount)
                throw new Exception($"Invalid agents counts. {Constants.PlayersCount} agents should be created.");
            _agents = agents;
            _dealerId = dealerId;
        }

        public int[] Run(int totalSteps)
        {
            var stats = new int[Constants.PlayersCount];
            using (var progressBar = new ProgressBar())
            {
                for (var step = 0; step < totalSteps; step++)
                {
                    progressBar.Report(((double) step / totalSteps, null));
                    var playersPoints = SimulateGame();
                    var winnerId = playersPoints
                        .ToList()
                        .IndexOf(playersPoints.Max());
                    stats[winnerId] += 1;
                }
            }

            return stats;
        }

        private int[] SimulateGame()
        {
            var gameState = new GameState(_dealerId);
            var initializerId = gameState.NextPlayerId;
            var cardsToLet = _agents[initializerId].GetCardsToLet(gameState.GetNextPlayerState());
            _agents.ForEach(agent => agent.Init(cardsToLet, initializerId, gameState.GetPlayerState(agent.PlayerId)));

            while (!gameState.GameFinished)
            {
                var availableCards = gameState.GetAvailableActions()
                    .Select(availableAction => availableAction.Card)
                    .ToArray();
                var action = _agents[gameState.NextPlayerId].GetAction(gameState.GetNextPlayerState(), availableCards);
                gameState = gameState.PerformAction(action);
            }

            return gameState.PlayersPoints;
        }
    }
}