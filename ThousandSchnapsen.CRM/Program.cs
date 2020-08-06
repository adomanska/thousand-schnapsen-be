using ThousandSchnapsen.CRM.Algorithms;

namespace ThousandSchnapsen.CRM
{
    class Program
    {
        static void Main()
        {
            var trainer = new CfrTrainer();
            trainer.Train(10);
            trainer.Save("./nodeData.dat");
            trainer.Load("./nodeData.dat");
            trainer.Train(10);
        }
    }
}