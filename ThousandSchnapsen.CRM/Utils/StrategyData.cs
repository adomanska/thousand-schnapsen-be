using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using ThousandSchnapsen.Common.Commons;

namespace ThousandSchnapsen.CRM.Utils
{
    public class StrategyData
    {
        private readonly Card[] _availableActions;
        private double[] _regretSum = new double[Constants.CardsCount];

        public StrategyData(IEnumerable<Card> availableActions)
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
        
        public double[] StrategySum { get; set; }
        
        public double[] Strategy { get; private set; }

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
            Strategy = new double[Constants.CardsCount];
            _availableActions.ForEach(card => Strategy[card.CardId] = 1.0 / _availableActions.Length);
        }
    }
}