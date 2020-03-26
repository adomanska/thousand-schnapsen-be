using System.Text.Json.Serialization;
using ThousandSchnapsen.Common.Commons;

namespace ThousandSchnapsen.Common.Interfaces
{
    public interface IPlayerState : IPublicState
    {
        CardsSet Cards { get; }
        int PlayerId { get; }
    }
}