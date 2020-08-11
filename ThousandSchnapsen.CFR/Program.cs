using System;
using ThousandSchnapsen.Common.Agents;
using ThousandSchnapsen.Common.Interfaces;
using ThousandSchnapsen.CFR.Algorithms;
using ThousandSchnapsen.CFR.Agents;
using ThousandSchnapsen.CFR.Utils;

namespace ThousandSchnapsen.CFR
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine(Pad("TRAINING IN PROGRESS", '-'));
            var trainer = new CfrTrainer();
            trainer.Train(1000);
            trainer.Save("./testData2");

            Console.WriteLine(Pad("EVALUATION IN PROGRESS", '-'));
            var cfrPlayer = new CfrAgent(1, new[] {0, 2},
                "./testData2");
            var agents = new IAgent[]
            {
                new RandomAgent(0),
                cfrPlayer,
                new RandomAgent(2),
                new RandomAgent(3)
            };
            var simulation = new Simulation(agents, 3);
            var stats = simulation.Run(1000);
            Console.WriteLine($"[{stats[0]} | {stats[1]} | {stats[2]}]");
        }

        private static string Pad(string content, char paddingChar, int lineWidth = 102)
        {
            var leftPadding = (lineWidth - content.Length) / 2;
            return content
                .PadRight(lineWidth - leftPadding, paddingChar)
                .PadLeft(lineWidth, paddingChar);
        }
    }
}