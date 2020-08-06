using System;
using System.Linq;
using MoreLinq;
using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.Common.States;
using ThousandSchnapsen.Common.Utils;
using Action = ThousandSchnapsen.Common.Commons.Action;

namespace ThousandSchnapsen.CRM.Utils
{
    public struct KnowledgeParams
    {
        public CardsSet[] PossibleCardsSets;
        public CardsSet[] CertainCardsSets;
        public byte[] CardsLeft;
        public int[] OpponentsIds;
        public int PlayerId;
    }
    
    public class Knowledge
    {
        private readonly CardsSet[] _possibleCardsSets;
        private readonly CardsSet[] _certainCardsSets;
        private readonly byte[] _cardsLeft;
        private readonly int[] _opponentsIds;
        private readonly int _playerId;

        public Knowledge(int playerId, int[] opponentsIds, CardsSet dealerCards, int initializerId, (int, int)[] cardsToLet)
        {
            _possibleCardsSets = new CardsSet[Constants.PlayersCount].Populate(_ => CardsSet.Deck());
            _certainCardsSets = new CardsSet[Constants.PlayersCount].Populate(_ => new CardsSet());
            _cardsLeft = new byte[Constants.PlayersCount].Populate(_ => (byte) Constants.CardsPerPlayerCount);
            _opponentsIds = opponentsIds;
            _playerId = playerId;

            _possibleCardsSets[initializerId] -= dealerCards;
            _certainCardsSets[initializerId] |= dealerCards;
            cardsToLet.ForEach((item, index) =>
            {
                var (opponentId, cardId) = item;

                _possibleCardsSets[initializerId].RemoveCard(cardId);
                _certainCardsSets[initializerId].RemoveCard(cardId);

                _possibleCardsSets[opponentId] -= dealerCards;
                _possibleCardsSets[opponentId].RemoveCard(cardId);
                _certainCardsSets[opponentId].AddCard(cardId);

                var (secondOpponentId, _) = cardsToLet[Math.Abs(index - 1)];
                _possibleCardsSets[secondOpponentId].RemoveCard(cardId);
            });
        }

        public Knowledge(KnowledgeParams knowledgeParams)
        {
            _possibleCardsSets = knowledgeParams.PossibleCardsSets;
            _certainCardsSets = knowledgeParams.CertainCardsSets;
            _cardsLeft = knowledgeParams.CardsLeft;
            _opponentsIds = knowledgeParams.OpponentsIds;
            _playerId = knowledgeParams.PlayerId;
        }

        public InfoSet GetInfoSet(CardsSet playerCardsSet, Card[] availableActions)
        {
            var availableCardsSet = new CardsSet(availableActions);

            return new InfoSet(playerCardsSet, availableCardsSet, _opponentsIds,
                _possibleCardsSets, _certainCardsSets, _cardsLeft, _playerId);
        }

        public Knowledge GetNext(Action action, PublicState gameState, bool trump)
        {
            var nextPossibleCardsSets = _possibleCardsSets.Select(cardsSet => cardsSet.Clone()).ToArray();
            var nextCertainCardsSets = _certainCardsSets.Select(cardsSet => cardsSet.Clone()).ToArray();
            var nextCardsLeft = new byte[Constants.PlayersCount];
            Array.Copy(_cardsLeft, nextCardsLeft, _cardsLeft.Length);
            nextCardsLeft[action.PlayerId] -= 1;

            if(trump)
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

            if (!trump && action.Card.IsPartOfMarriage && gameState.StockEmpty && action.Card.SecondMarriagePart.HasValue)
                nextPossibleCardsSets[action.PlayerId].RemoveCard(action.Card.SecondMarriagePart.Value);

            nextPossibleCardsSets.ForEach(cardsSet => cardsSet.RemoveCard(action.Card));
            nextCertainCardsSets[action.PlayerId].RemoveCard(action.Card);
            nextPossibleCardsSets[action.PlayerId] -= GetImpossibleCardsSet(action.Card, gameState);

            return new Knowledge(new KnowledgeParams()
            {
                PossibleCardsSets = nextPossibleCardsSets,
                CertainCardsSets = nextCertainCardsSets,
                CardsLeft = nextCardsLeft,
                OpponentsIds = _opponentsIds,
                PlayerId = _playerId
            });
        }

        private CardsSet GetImpossibleCardsSet(Card card, PublicState gameState)
        {
            if (gameState.StockEmpty)
                return new CardsSet();

            var stockColor = gameState.Stock.First().Card.Color;
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