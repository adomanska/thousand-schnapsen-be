using System.Linq;

namespace ThousandSchnapsen.CRM
{
    public class Node
    {
        private const int NumActions = 24;

        private readonly double[] _strategy = new double[NumActions];
        private readonly double[] _strategySum = new double[NumActions];
        private readonly byte[] _availableActions;

        public Node(byte[] availableActions) =>
            _availableActions = availableActions;

        public double[] RegretSum { get; } = new double[NumActions];

        public double[] GetStrategy(double realizationWeight)
        {
            double normalizingSum = 0;
            foreach (var a in _availableActions)
            {
                _strategy[a] = RegretSum[a] > 0 ? RegretSum[a] : 0;
                normalizingSum += _strategy[a];
            }

            foreach (var a in _availableActions)
            {
                if (normalizingSum > 0)
                    _strategy[a] /= normalizingSum;
                else
                    _strategy[a] = 1.0 / _availableActions.Length;
                _strategySum[a] += realizationWeight * _strategy[a];
            }

            return _strategy;
        }

        public double[] GetAverageStrategy()
        {
            var avgStrategy = new double[NumActions];
            var normalizingSum = _availableActions.Sum(a => _strategySum[a]);
            foreach (var a in _availableActions)
                if (normalizingSum > 0)
                    avgStrategy[a] = _strategySum[a] / normalizingSum;
                else
                    avgStrategy[a] = 1.0 / NumActions;
            return avgStrategy;
        }
    }
}