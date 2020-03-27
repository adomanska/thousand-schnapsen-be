using ThousandSchnapsen.Common.Commons;

namespace ThousandSchnapsen.Common.States
{
    public class PlayerState : PublicState
    {
        public int PlayerId { get; set; }
        public CardsSet Cards { get; set; }
    }
}