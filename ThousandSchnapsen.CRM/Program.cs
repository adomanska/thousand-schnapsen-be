using ThousandSchnapsen.CRM.Algo;

namespace ThousandSchnapsen.CRM
{
    class Program
    {
        static void Main()
        {
            var trainer = new Trainer();
            trainer.Train(100_000);
        }
    }
}