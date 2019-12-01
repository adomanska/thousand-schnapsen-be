namespace ThousandSchnapsen.Common
{
    public class Action
    {
        public Action(int playerId, int cardId)
        {
            PlayerId = playerId;
            CardId = cardId;
        }

        public int PlayerId { get; }
        public int CardId { get; }

        public void Deconstruct(out int playerId, out int cardId)
        {
            playerId = PlayerId;
            cardId = CardId;
        }
    }
}