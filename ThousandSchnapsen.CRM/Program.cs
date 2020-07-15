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
            var trainer = new Trainer();
            trainer.Train(100_000);
        }
    }
}