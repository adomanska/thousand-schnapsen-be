using System;

namespace ThousandSchnapsen.Common.Commons
{
    public class Action
    {
        public int PlayerId { get; }
        public Card Card { get; }

        public Action(int playerId, Card card)
        {
            PlayerId = playerId;
            Card = card;
        }

        public void Deconstruct(out int playerId, out Card card)
        {
            playerId = PlayerId;
            card = Card;
        }

        public override bool Equals(object obj) =>
            Equals(obj as Action);

        public override int GetHashCode() =>
            HashCode.Combine(PlayerId, Card);

        private bool Equals(Action other) =>
            PlayerId == other.PlayerId && Card.Equals(other.Card);
    }
}