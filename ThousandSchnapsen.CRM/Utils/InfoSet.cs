using System.Linq;
using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.Common.Utils;

namespace ThousandSchnapsen.CRM.Utils
{
    public class InfoSet
    {
        private readonly CardsSet _playerCardsSet;
        private readonly CardsSet[] _opponentsCardsSets;
        private readonly int[] _opponentsIds;
        private readonly int _playerId;
        public InfoSet(CardsSet playerCardsSet, CardsSet availableCardsSet, int[] opponentsIds,
            CardsSet[] possibleCardsSets, CardsSet[] certainCardsSets, byte[] cardsLeft, int playerId)
        {
            var opponentsPossibleCardsSet = (possibleCardsSets[opponentsIds[0]] & possibleCardsSets[opponentsIds[1]])
                                            - playerCardsSet;

            var opponentsCertainCardsSets = new[]
            {
                (certainCardsSets[opponentsIds[0]] |
                 (possibleCardsSets[opponentsIds[0]] - playerCardsSet - opponentsPossibleCardsSet)),
                (certainCardsSets[opponentsIds[1]] |
                 (possibleCardsSets[opponentsIds[1]] - playerCardsSet - opponentsPossibleCardsSet))
            };

            if (opponentsCertainCardsSets[0].Count == cardsLeft[opponentsIds[0]] && !opponentsPossibleCardsSet.IsEmpty)
            {
                opponentsCertainCardsSets[1] |= opponentsPossibleCardsSet;
                opponentsPossibleCardsSet = new CardsSet();
            }
            else if (opponentsCertainCardsSets[1].Count == cardsLeft[opponentsIds[1]] && !opponentsPossibleCardsSet.IsEmpty)
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
                opponentsPossibleCardsSet.Code,
                opponentsCertainCardsSetsCodes[0],
                opponentsCertainCardsSetsCodes[1]
            });

            RawData = (
                data[0],
                data[1],
                data[2],
                data[3]
            );
            _playerCardsSet = playerCardsSet;
            _opponentsIds = opponentsIds;
            _opponentsCardsSets = opponentsCertainCardsSets;
            _playerId = playerId;
        }

        public (int, int, int, int) RawData;

        public bool IsCertain => RawData.Item2 == 0;

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