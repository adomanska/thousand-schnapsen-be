﻿using System;
using ThousandSchnapsen.Common.Agents;
using ThousandSchnapsen.Common.Interfaces;
using ThousandSchnapsen.CRM.Agents;
using ThousandSchnapsen.CRM.Algorithms;
using ThousandSchnapsen.CRM.Utils;

namespace ThousandSchnapsen.CRM
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine(Pad("TRAINING IN PROGRESS", '-'));
            var trainer = new CfrTrainer();
            trainer.Train(10_000);
            trainer.Save("./nodeData.dat");
            
            Console.WriteLine(Pad("EVALUATION IN PROGRESS",'-'));
            var agents = new IAgent[]
            {
                new CfrAgent(0, new [] {1, 2}, "./nodeData.dat"), 
                new RandomAgent(1),
                new RandomAgent(2),
                new RandomAgent(3)
            };
            var simulation = new Simulation(agents, 3);
            var stats = simulation.Run(10_000);
            Console.WriteLine(stats[0]);
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