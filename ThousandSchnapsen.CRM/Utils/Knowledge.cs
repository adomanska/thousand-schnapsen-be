using System;
using System.Linq;
using MoreLinq;
using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.Common.States;
using ThousandSchnapsen.Common.Utils;
using Action = ThousandSchnapsen.Common.Commons.Action;

namespace ThousandSchnapsen.CRM.Utils
{
    public class Knowledge
    {
        public CardsSet[] PossibleCardsSets;
        public CardsSet[] CertainCardsSets;
        public byte[] CardsLeft;
        public bool StockEmpty;

        private Knowledge()
        {
        }

        public Knowledge(CardsSet dealerCards, int initializerId, (int, byte)[] cardsToLet)
        {
            PossibleCardsSets = new CardsSet[Constants.PlayersCount].Populate(_ => CardsSet.Deck());
            CertainCardsSets = new CardsSet[Constants.PlayersCount].Populate(_ => new CardsSet());
            CardsLeft = new byte[Constants.PlayersCount].Populate(_ => (byte) Constants.CardsPerPlayerCount);
            StockEmpty = true;

            PossibleCardsSets[initializerId] -= dealerCards;
            CertainCardsSets[initializerId] |= dealerCards;
            cardsToLet.ForEach((item, index) =>
            {
                var (opponentId, cardId) = item;

                PossibleCardsSets[initializerId].RemoveCard(cardId);
                CertainCardsSets[initializerId].RemoveCard(cardId);

                PossibleCardsSets[opponentId] -= dealerCards;
                PossibleCardsSets[opponentId].RemoveCard(cardId);
                CertainCardsSets[opponentId].AddCard(cardId);

                var (secondOpponentId, _) = cardsToLet[Math.Abs(index - 1)];
                PossibleCardsSets[secondOpponentId].RemoveCard(cardId);
            });
        }

        public InfoSet GetInfoSet(CardsSet playerCardsSet, Card[] availableActions, int playerId, int[] opponentsIds)
        {
            var availableCardsSet = new CardsSet(availableActions);

            return new InfoSet(playerCardsSet, availableCardsSet, opponentsIds,
                PossibleCardsSets, CertainCardsSets, CardsLeft, playerId, StockEmpty ? 1 : 0);
        }

        public Knowledge GetNext(Action action, PublicState gameState, bool trump, bool stockEmpty)
        {
            var nextPossibleCardsSets = PossibleCardsSets.Select(cardsSet => cardsSet.Clone()).ToArray();
            var nextCertainCardsSets = CertainCardsSets.Select(cardsSet => cardsSet.Clone()).ToArray();
            var nextCardsLeft = new byte[Constants.PlayersCount];
            Array.Copy(CardsLeft, nextCardsLeft, CardsLeft.Length);
            nextCardsLeft[action.PlayerId] -= 1;

            if (trump)
            {
                if (action.Card.SecondMarriagePart.HasValue)
                {
                    nextCertainCardsSets[action.PlayerId].AddCard(action.Card.SecondMarriagePart.Value);
                    nextPossibleCardsSets[action.PlayerId].RemoveCard(action.Card.SecondMarriagePart.Value);
                    Enumerable.Range(0, Constants.PlayersCount)
                        .Where(playerId => playerId != action.PlayerId)
                        .ForEach(playerId =>
                            nextPossibleCardsSets[playerId].RemoveCard(action.Card.SecondMarriagePart.Value)
                        );
                }
            }

            if (!trump && action.Card.IsPartOfMarriage && gameState.StockEmpty &&
                action.Card.SecondMarriagePart.HasValue)
                nextPossibleCardsSets[action.PlayerId].RemoveCard(action.Card.SecondMarriagePart.Value);

            nextPossibleCardsSets.ForEach(cardsSet => cardsSet.RemoveCard(action.Card));
            nextCertainCardsSets[action.PlayerId].RemoveCard(action.Card);
            nextPossibleCardsSets[action.PlayerId] -= GetImpossibleCardsSet(action.Card, gameState);

            return new Knowledge()
            {
                PossibleCardsSets = nextPossibleCardsSets,
                CertainCardsSets = nextCertainCardsSets,
                CardsLeft = nextCardsLeft,
                StockEmpty = stockEmpty
            };
        }

        private CardsSet GetImpossibleCardsSet(Card card, PublicState gameState)
        {
            if (gameState.StockEmpty)
                return new CardsSet();
            var stockColor = gameState.StockColor;
            var trumpColor = gameState.Trump;
            var maxStockCard = gameState.Stock
                .MaxBy(stockItem => stockItem.Card.GetValue(stockColor, trumpColor))
                .First().Card;
            var maxStockValue = maxStockCard.GetValue(stockColor, trumpColor);

            var stockColorCardsSet = CardsSet.Color(stockColor);
            var greaterCardsSet = CardsSet.GetHigherCardsSet(maxStockCard, stockColor, trumpColor);

            if (card.Color == stockColor && card.GetValue(stockColor, trumpColor) < maxStockValue)
                return stockColorCardsSet & greaterCardsSet;
            if (card.Color != stockColor && card.GetValue(stockColor, trumpColor) > maxStockValue)
                return stockColorCardsSet;
            if (card.Color != stockColor && card.GetValue(stockColor, trumpColor) < maxStockValue)
                return stockColorCardsSet | greaterCardsSet;

            return new CardsSet();
        }
    }
}