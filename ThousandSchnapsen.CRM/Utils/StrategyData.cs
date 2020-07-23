using System;
using System.Collections.Generic;
using System.Linq;
using ThousandSchnapsen.Common.Utils;

namespace ThousandSchnapsen.CRM.Utils
{
    public class StrategyData
    {
        private readonly byte[] _availableActions;
        private float[] _regretSum;

        public StrategyData(IEnumerable<byte> availableActions)
        {
            _availableActions = availableActions.ToArray();
            _regretSum = new float[_availableActions.Length];
            StrategySum = new float[_availableActions.Length];
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

        public void UpdateStrategy()
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
            Strategy = new float[_availableActions.Length]
                .Populate(_ => 1f / _availableActions.Length);
        }
    }
}