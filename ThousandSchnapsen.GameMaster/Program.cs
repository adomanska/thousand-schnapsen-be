using ThousandSchnapsen.Common;

namespace ThousandSchnapsen.GameMaster
{
    class Program
    {
        static void Main(string[] args)
        {
            ILogger logger = new Logger();
            IGameState gameState = new GameState(3);

            logger.Log(gameState);
            while(!gameState.GameFinished)
            {
                var action = gameState.GetAvailableActions()[0];
                gameState.PerformAction(action);
                logger.Log(gameState);
            }
        }
    }
}
