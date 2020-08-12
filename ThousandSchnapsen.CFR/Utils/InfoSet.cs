#nullable enable
using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.Common.States;

namespace ThousandSchnapsen.CFR.Utils
{
    public class InfoSet
    {
        public InfoSet(PlayerState playerState, CardsSet availableCardsSet, int[] opponentsIds,
            CardsSet[] possibleCardsSets, CardsSet[] certainCardsSets, byte[] cardsLeft)
        {
            var opponentsPossibleCardsSet = (possibleCardsSets[opponentsIds[0]] & possibleCardsSets[opponentsIds[1]])
                                            - playerState.Cards;

            var opponentsCertainCardsSets = new[]
            {
                (certainCardsSets[opponentsIds[0]] |
                 (possibleCardsSets[opponentsIds[0]] - playerState.Cards - opponentsPossibleCardsSet)),
                (certainCardsSets[opponentsIds[1]] |
                 (possibleCardsSets[opponentsIds[1]] - playerState.Cards - opponentsPossibleCardsSet))
            };

            if (opponentsCertainCardsSets[0].Count == cardsLeft[opponentsIds[0]] && !opponentsPossibleCardsSet.IsEmpty)
            {
                opponentsCertainCardsSets[1] |= opponentsPossibleCardsSet;
                opponentsPossibleCardsSet = new CardsSet();
            }
            else if (opponentsCertainCardsSets[1].Count == cardsLeft[opponentsIds[1]] &&
                     !opponentsPossibleCardsSet.IsEmpty)
            {
                opponentsCertainCardsSets[0] |= opponentsPossibleCardsSet;
                opponentsPossibleCardsSet = new CardsSet();
            }

            RawData = (
                availableCardsSet.Code,
                (playerState.Cards - availableCardsSet).Code,
                opponentsPossibleCardsSet.Code, 
                opponentsCertainCardsSets[0].Code,
                opponentsCertainCardsSets[1].Code
            );
            IsCertain = opponentsPossibleCardsSet.IsEmpty;
            if (IsCertain)
            {
                var playersCards = new CardsSet[Constants.PlayersCount];
                playersCards[playerState.PlayerId] = playerState.Cards;
                playersCards[opponentsIds[0]] = opponentsCertainCardsSets[0];
                playersCards[opponentsIds[1]] = opponentsCertainCardsSets[1];
                playersCards[playerState.DealerId] = playerState.DealerCards;
                PlayersCards = playersCards;
            }
        }

        public (int, int, int, int, int) RawData;

        public bool IsCertain { get; }

        public CardsSet[]? PlayersCards { get; }
    }
}