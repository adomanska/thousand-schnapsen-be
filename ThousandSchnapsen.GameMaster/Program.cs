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
            ILogger logger = new Logger();
            IGameState gameState = new GameState(1);
            IAgent[] agents =
            {
                new RandomAgent(0),
                new RandomAgent(1),
                new FixedAgent(2),
                new RandomAgent(3)
            };

            logger.Log(gameState);
            while (!gameState.GameFinished)
            {
                Thread.Sleep(500);
                var action = agents[gameState.NextPlayerId].GetAction(gameState.GetPlayerState(gameState.NextPlayerId));
                gameState.PerformAction(action);
                logger.Log(gameState);
            }
        }
    }
}