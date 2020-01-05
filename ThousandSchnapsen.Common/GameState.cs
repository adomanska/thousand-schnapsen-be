using System.Linq;
using System;
using System.Text;
using MoreLinq;

namespace ThousandSchnapsen.Common
{
    public class GameState : PublicState
    {
        const int PLAYERS_COUNT = 4;
        const int CARDS_IN_COLOR_COUNT = 6;
        const int COLORS_COUNT = 4;
        const int CARDS_PER_PLAYER_COUNT = 7;
        const int REST_CARDS_COUNT = 3;

        public CardsSet[] PlayersCards { get; set; }
        public bool GameFinished => PlayersCards.Select((playerCards, index) => index == DealerId || playerCards.IsEmpty)
            .All(finished => finished);

        public static GameState RandomState(int dealerId)
        {
            var random = new Random();
            Color[] colors = new Color[]{
                Color.Spades,
                Color.Clubs,
                Color.Diamonds,
                Color.Hearts
            };
            Rank[] ranks = new Rank[]{
                Rank.Nine,
                Rank.Ten,
                Rank.Jack,
                Rank.Queen,
                Rank.King,
                Rank.Ace
            };
            var deck = colors.SelectMany(color => ranks.Select(rank => new Card(rank, color)));
            var shuffledDeck = deck.OrderBy(Card => random.Next()).Select(card => card.CardId).ToArray();
            CardsSet[] playersCards = new CardsSet[PLAYERS_COUNT];
            int start = 0;
            for (int playerId = 0; playerId < PLAYERS_COUNT; playerId++)
                if (playerId != dealerId)
                {
                    playersCards[playerId] = new CardsSet(shuffledDeck.Slice(start, CARDS_PER_PLAYER_COUNT).ToArray());
                    start += CARDS_PER_PLAYER_COUNT;
                }          
            playersCards[dealerId] = new CardsSet(shuffledDeck.Slice(start, REST_CARDS_COUNT).ToArray());

            return new GameState(){
                Stock = new (int PlayerId, Card Card)[]{},
                PlayersUsedCards = Enumerable.Range(0, PLAYERS_COUNT).Select(playerId => new CardsSet()).ToArray(),
                PlayersPoints = new int[PLAYERS_COUNT],
                TrumpsHistory = new Color[]{},
                DealerId = dealerId,
                NextPlayerId = (dealerId + 1) % PLAYERS_COUNT,
                PlayersCards = playersCards
            };
        }

        public PlayerState GetPlayerState(int playerId)
        {
            var stock = Stock.Select(pc => (pc.PlayerId, pc.Card.Clone())).ToArray();
            var playersUsedCards = PlayersUsedCards.Select(cs => cs.Clone()).ToArray();
            var playersPoints = (int[])this.PlayersPoints.Clone();
            var trumpsHistory = (Color[])(this.TrumpsHistory.Clone());
            var cards = PlayersCards[playerId].Clone();

            return new PlayerState()
            {
                Stock = stock,
                PlayersUsedCards = playersUsedCards,
                PlayersPoints = playersPoints,
                TrumpsHistory = trumpsHistory,
                Cards = cards,
                PlayerId = playerId
            };
        }

        public void PerformAction(Action action)
        {
            var (playerId, cardId) = action;

            if (playerId != NextPlayerId)
                throw new InvalidOperationException();

            MoveCard(cardId);
            if (Stock.Length == 1)
            {
                UpdateTrumpsAndPoints(cardId);
                NextPlayerId = GetNextPlayerId(NextPlayerId);
            }
            else if (Stock.Length == 2)
                NextPlayerId = GetNextPlayerId(NextPlayerId);
            else
                EvaluateTurn();
        }

        public Action[] GetAvailableActions()
        {
            CardsSet availableCards;
            CardsSet playerCards = PlayersCards[NextPlayerId];
            if (Stock.Length == 0)
            {
                availableCards = playerCards;
            }
            else
            {
                Color stockColor = Stock[0].Card.Color;
                int baseMask = (int)Math.Pow(2, CARDS_IN_COLOR_COUNT) - 1;
                int colorMask = baseMask << (CARDS_IN_COLOR_COUNT * (int)stockColor);
                availableCards = playerCards & new CardsSet(colorMask);
                if (availableCards.IsEmpty)
                    availableCards = playerCards;
            }
            return availableCards.GetCardsIds().Select(cardId => new Action
                {
                    CardId = cardId,
                    PlayerId = NextPlayerId
                }).ToArray();
        }

        public override string ToString()
        {
            const int lineWidth = 42;
            var sb = new StringBuilder();
            sb.AppendLine(Utils.CreateTitle("GAME STATE", lineWidth));
            sb.AppendLine(base.ToString());
            sb.AppendLine("PLAYERS CARDS:");
            Func<int, string> playerSymbol = playerId => playerId == DealerId ? "(D)" : (playerId == NextPlayerId ? " ->" : "   ");
            for (int playerId = 0; playerId < PLAYERS_COUNT; playerId++)
                sb.AppendLine($"{playerSymbol(playerId)} {playerId + 1}:  {PlayersCards[playerId]}");
            return sb.ToString();
        }

        private void MoveCard(int cardId)
        {
            if (Stock.Length == PLAYERS_COUNT - 1)
                Stock = new (int PlayerId, Card Card)[]{};
            PlayersCards[NextPlayerId].RemoveCard(cardId);
            PlayersUsedCards[NextPlayerId].AddCard(cardId);
            Stock = Stock.Concat(new (int PlayerId, Card Card)[] { (PlayerId: NextPlayerId, Card: new Card(cardId)) }).ToArray();
        }

        private void UpdateTrumpsAndPoints(int cardId)
        {
            Card card = new Card(cardId);

            if (card.IsPartOfMarriage && PlayersCards[NextPlayerId].Contains(card.SecondMarriagePart))
            {
                TrumpsHistory = TrumpsHistory.Concat(new Color[] { card.Color }).ToArray();
                PlayersPoints[NextPlayerId] += card.Color.GetPoints();
            }
        }

        private void EvaluateTurn()
        {
            Color firstColor = Stock[0].Card.Color;
            int points = Stock.Sum(stockItem => stockItem.Card.Rank.GetPoints());
            (NextPlayerId, _) = Stock.MaxBy(stockItem => stockItem.Card.GetValue(firstColor, Trump)).First();
            PlayersPoints[NextPlayerId] += points;
        }

        private int GetNextPlayerId(int playerId)
        {
            int nextPlayerId = (playerId + 1) % PLAYERS_COUNT;
            if (nextPlayerId == DealerId)
            {
                nextPlayerId = (nextPlayerId + 1) % PLAYERS_COUNT;
            }
            return nextPlayerId;
        }
    }
}