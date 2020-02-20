namespace ThousandSchnapsen.Common
{
    public interface IPlayerState: IPublicState
    {
        CardsSet Cards { get; }
        int PlayerId { get; }
    }
}