using System;
using ThousandSchnapsen.Common;

namespace ThousandSchnapsen.GameMaster
{
    class Program
    {
        static void Main(string[] args)
        {
            GameState gameState = GameState.RandomState(0);
            Console.WriteLine(gameState);
        }
    }
}
