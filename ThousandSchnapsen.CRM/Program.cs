using ThousandSchnapsen.CRM.Algorithms;

namespace ThousandSchnapsen.CRM
{
    class Program
    {
        static void Main()
        {
            var trainer = new CfrTrainer();
            trainer.Train(100_000);
        }
    }
}