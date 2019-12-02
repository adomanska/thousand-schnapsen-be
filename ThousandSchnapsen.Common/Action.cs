namespace ThousandSchnapsen.Common
{
    public class Action
    {
        public int PlayerId { get; set; }
        public int CardId { get;  set; }

        public void Deconstruct(out int playerId, out int cardId)
        {
            playerId = PlayerId;
            cardId = CardId;
        }
    }
}