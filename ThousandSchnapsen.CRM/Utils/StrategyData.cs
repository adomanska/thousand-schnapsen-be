using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using ThousandSchnapsen.Common.Commons;

namespace ThousandSchnapsen.CRM.Utils
{
    public class StrategyData
    {
        private readonly int[] _availableActions;
        private double[] _regretSum = new double[Constants.CardsCount];

        public StrategyData(IEnumerable<int> availableActions)
        {
            _availableActions = availableActions.ToArray();
            ResetStrategy();
        }

        public double[] RegretSum
        {
            get => _regretSum;
            set
            {
                _regretSum = value;
                UpdateStrategy();
            }
        }
        
        public double[] StrategySum { get; } = new double[Constants.CardsCount];
        
        public double[] Strategy { get; private set; }

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
            Strategy = new double[Constants.CardsCount];
            _availableActions.ForEach(cardId => Strategy[cardId] = 1.0 / _availableActions.Length);
        }
    }
}