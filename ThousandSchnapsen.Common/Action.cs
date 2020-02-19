namespace ThousandSchnapsen.Common
{
    public class Action
    {
        public int PlayerId { get; set; }
        public Card Card { get;  set; }

        public void Deconstruct(out int playerId, out Card card)
        {
            playerId = PlayerId;
            card = Card;
        }
    }
}