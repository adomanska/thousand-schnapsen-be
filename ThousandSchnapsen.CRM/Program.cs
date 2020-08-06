using System;
using ThousandSchnapsen.Common.Agents;
using ThousandSchnapsen.Common.Interfaces;
using ThousandSchnapsen.CRM.Agents;
using ThousandSchnapsen.CRM.Utils;

namespace ThousandSchnapsen.CRM
{
    class Program
    {
        static void Main()
        {
            var agents = new IAgent[]
            {
                new CfrAgent(0, new [] {1, 2}, "./nodeData.dat"), 
                new RandomAgent(1),
                new RandomAgent(2),
                new RandomAgent(3)
            };
            var simulation = new Simulation(agents, 3);
            var stats = simulation.Run(10000);
            Console.WriteLine(stats[0]);
        }
    }
}