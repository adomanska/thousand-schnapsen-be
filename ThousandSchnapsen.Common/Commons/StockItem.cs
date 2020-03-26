using System;

namespace ThousandSchnapsen.Common.Commons
{
    public struct StockItem
    {
        public int PlayerId { get; }
        public Card Card { get; }

        public StockItem(int playerId, Card card)
        {
            PlayerId = playerId;
            Card = card;
        }

        public void Deconstruct(out int playerId, out Card card)
        {
            playerId = PlayerId;
            card = Card;
        }
    }
}