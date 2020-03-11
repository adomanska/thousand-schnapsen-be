using System;
using System.Linq;
using System.Threading;
using ThousandSchnapsen.Common.Agents;
using ThousandSchnapsen.Common.Interfaces;
using ThousandSchnapsen.Common.Loggers;
using ThousandSchnapsen.Common.States;

namespace ThousandSchnapsen.GameMaster
{
    class Program
    {
        static void Main(string[] args)
        {
            IAgent[] agentsWithFixed =
            {
                new RandomAgent(0),
                new RandomAgent(1),
                new FixedAgent(2),
                new RandomAgent(3)
            };
            IAgent[] randomAgents =
            {
                new RandomAgent(0),
                new RandomAgent(1),
                new RandomAgent(2),
                new RandomAgent(3)
            };
            
            PerformSimulation(agentsWithFixed, 1);
            
            // PerformTest(agentsWithFixed, 1);
            // PerformTest(agentsWithFixed, 0);
            // PerformTest(randomAgents, 1);
            // PerformTest(randomAgents, 0);

        }

        private static void PerformSimulation(IAgent[] agents, int dealerId)
        {
            ILogger logger = new Logger();
            IGameState gameState = new GameState(dealerId);
            logger.Log(gameState);
            while (!gameState.GameFinished)
            {
                Thread.Sleep(500);
                var action = agents[gameState.NextPlayerId].GetAction(gameState.GetPlayerState(gameState.NextPlayerId));
                gameState.PerformAction(action);
                logger.Log(gameState);
            }
        }

        private static void PerformTest(IAgent[] agents, int dealerId, int examinatedPlayerId = 2)
        {
            const int n = 100000;
            var wins = 0;
            for (var i = 0; i < n; i++)
            {
                IGameState gameState = new GameState(dealerId);
                while (!gameState.GameFinished)
                {
                    var action = agents[gameState.NextPlayerId].GetAction(gameState.GetPlayerState(gameState.NextPlayerId));
                    gameState.PerformAction(action);
                }

                if (gameState.PlayersPoints[examinatedPlayerId] == gameState.PlayersPoints.Max())
                    wins += 1;
            }
            
            Console.WriteLine($"Result: {wins}/{n}");
        }
    }
}