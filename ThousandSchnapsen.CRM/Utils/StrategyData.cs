using System;
using System.Linq;
using ThousandSchnapsen.Common.Utils;

namespace ThousandSchnapsen.CRM.Utils
{
    public class StrategyData
    {
        private readonly int _availableActionsCount;
        private float[] _regretSum;

        public StrategyData(int availableActionsCount)
        {
            _availableActionsCount = availableActionsCount;
            _regretSum = new float[availableActionsCount];
            StrategySum = new float[availableActionsCount];
            ResetStrategy();
        }

        public float[] RegretSum
        {
            get => _regretSum;
            set
            {
                _regretSum = value;
                UpdateStrategy();
            }
        }

        public float[] StrategySum { get; set; }

        public float[] Strategy { get; private set; }

        private void UpdateStrategy()
        {
            Strategy = RegretSum
                .Select(regretSum => Math.Max(0, regretSum))
                .ToArray();
            var normalizingSum = Strategy.Sum();

            if (normalizingSum > 0)
                Strategy = Strategy
                    .Select(strategy => strategy / normalizingSum)
                    .ToArray();
            else
                ResetStrategy();
        }

        private void ResetStrategy()
        {
            Strategy = new float[_availableActionsCount]
                .Populate(_ => 1f / _availableActionsCount);
        }
    }
}
