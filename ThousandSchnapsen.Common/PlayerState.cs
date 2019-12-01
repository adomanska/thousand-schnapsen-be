namespace ThousandSchnapsen.Common
{
    public class PlayerState : PublicState
    {
        public int PlayerId { get; set; }
        public CardsSet Cards { get; set; }
    }
}