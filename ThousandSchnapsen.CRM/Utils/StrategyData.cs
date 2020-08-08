using System;
using System.Linq;
using System.Runtime.Serialization;
using ThousandSchnapsen.Common.Utils;

namespace ThousandSchnapsen.CRM.Utils
{
    [Serializable]
    public class StrategyData
    {
        [NonSerialized] private int _availableActionsCount;
        [NonSerialized] private float[] _regretSum;
        private float[] _strategySum;

        public StrategyData()
        {
        }

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

        public float[] StrategySum
        {
            get => _strategySum;
            set => _strategySum = value;
        }

        public float[] Strategy { get; private set; }

        public float[] AverageStrategy { get; private set; }

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

        [OnDeserialized]
        private void SetValuesOnDeserialized(StreamingContext context)
        {
            var strategy = StrategySum
                .Select(strategySum => Math.Max(0, strategySum))
                .ToArray();
            var normalizingSum = strategy.Sum();

            if (normalizingSum > 0)
                AverageStrategy = strategy
                    .Select(actionStrategy => actionStrategy / normalizingSum)
                    .ToArray();
            AverageStrategy = new float[StrategySum.Length]
                .Populate(_ => 1f / StrategySum.Length);
        }
    }
}