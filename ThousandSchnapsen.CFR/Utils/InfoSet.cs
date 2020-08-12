using System.Linq;
using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.Common.States;
using ThousandSchnapsen.Common.Utils;

namespace ThousandSchnapsen.CFR.Utils
{
    public class InfoSet
    {
        private readonly CardsSet _playerCardsSet;
        private readonly CardsSet[] _opponentsCardsSets;
        private readonly int[] _opponentsIds;
        private readonly int _playerId;

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

            var opponentsCertainCardsSetsCodes = opponentsCertainCardsSets
                .OrderBy(cardsSet => cardsSet.Code)
                .Select(cardsSet => cardsSet.Code)
                .ToArray();

            var data = CodeUnification.Unify(new[]
            {
                availableCardsSet.Code,
                opponentsCertainCardsSetsCodes[0],
                opponentsCertainCardsSetsCodes[1]
            });

            RawData = (
                data[0],
                (playerState.StockEmpty ? 1 : 0,
                    data[1],
                    data[2])
            );
            _playerCardsSet = playerState.Cards;
            _opponentsIds = opponentsIds;
            _opponentsCardsSets = opponentsCertainCardsSets;
            _playerId = playerState.PlayerId;
            IsCertain = opponentsPossibleCardsSet.IsEmpty;
        }

        public (int, (int, int, int)) RawData;

        public bool IsCertain { get; }

        public CardsSet[] PlayersCards
        {
            get
            {
                var playersCardsSets = new CardsSet[Constants.PlayersCount];
                playersCardsSets[_playerId] = _playerCardsSet;
                playersCardsSets[_opponentsIds[0]] = _opponentsCardsSets[0];
                playersCardsSets[_opponentsIds[1]] = _opponentsCardsSets[1];

                return playersCardsSets;
            }
        }
    }
}