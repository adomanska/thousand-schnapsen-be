using System;
using ThousandSchnapsen.Common;

namespace ThousandSchnapsen.GameMaster
{
    class Program
    {
        static void Main(string[] args)
        {
            GameState gameState = GameState.RandomState(0);
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine(gameState);
            while(!gameState.GameFinished)
            {
                var action = gameState.GetAvailableActions()[0];
                gameState.PerformAction(action);
                Console.WriteLine(gameState);
            }
        }
    }
}
