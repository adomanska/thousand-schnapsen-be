namespace ThousandSchnapsen.KuhnPokerCrm
{
    public class Node
    {
        private const int NumActions = 2;
        public string InfoSet { get; }

        private readonly double[] _strategy = new double[NumActions],
            _strategySum = new double[NumActions];

        public Node(string infoSet) =>
            InfoSet = infoSet;
        
        public double[] RegretSum { get; } = new double[NumActions];

        public double[] GetStrategy(double realizationWeight)
        {
            double normalizingSum = 0;
            for (var a = 0; a < NumActions; a++)
            {
                _strategy[a] = RegretSum[a] > 0 ? RegretSum[a] : 0;
                normalizingSum += _strategy[a];
            }

            for (var a = 0; a < NumActions; a++)
            {
                if (normalizingSum > 0)
                    _strategy[a] /= normalizingSum;
                else
                    _strategy[a] = 1.0 / NumActions;
                _strategySum[a] += realizationWeight * _strategy[a];
            }

            return _strategy;
        }

        public double[] GetAverageStrategy()
        {
            var avgStrategy = new double[NumActions];
            double normalizingSum = 0;
            for (var a = 0; a < NumActions; a++)
                normalizingSum += _strategySum[a];
            for (var a = 0; a < NumActions; a++)
                if (normalizingSum > 0)
                    avgStrategy[a] = _strategySum[a] / normalizingSum;
                else
                    avgStrategy[a] = 1.0 / NumActions;
            return avgStrategy;
        }
    }
}