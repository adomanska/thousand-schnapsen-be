using System;

namespace ThousandSchnapsen.KuhnPokerCrm
{
    class Program
    {
        static void Main(string[] args)
        {
            var trainer = new KuhnTrainer();
            trainer.Train(1000000);
        }
    }
}