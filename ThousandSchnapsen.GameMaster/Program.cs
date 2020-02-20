using ThousandSchnapsen.Common;
using System.Threading;

namespace ThousandSchnapsen.GameMaster
{
    class Program
    {
        static void Main(string[] args)
        {
            ILogger logger = new Logger();
            IGameState gameState = new GameState(1);

            logger.Log(gameState);
            while(!gameState.GameFinished)
            {
                Thread.Sleep(500);
                var action = gameState.GetAvailableActions()[0];
                gameState.PerformAction(action);
                logger.Log(gameState);
            }
        }
    }
}
