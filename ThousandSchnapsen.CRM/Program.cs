using System;
using System.Diagnostics;
using System.Timers;
using ThousandSchnapsen.Common.States;

namespace ThousandSchnapsen.CRM
{
    class Program
    {
        static void Main(string[] args)
        {
            // int size = (int) (Math.Pow(2, 24) - 1);
            // bool [,] table = new bool[size, size];
            // Console.WriteLine(size);
            // Console.WriteLine(table[200, 200]);
            // var trainer = new Trainer();
            // trainer.Train(100_000);
            // Console.WriteLine("Nodes created: " + trainer.nodesCount);
            const int N = 100;
            var timer = new Stopwatch();
            
            timer.Start();
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    GameState gameState = new GameState(3);
                    var root = new ThousandSchnapsen.Common.GameTree.Node(gameState);
                    root.Expand(0);
                }
            }
            timer.Stop();
            
            Console.WriteLine($"Elapsed time {timer.Elapsed}");
        }
    }
}