using ThousandSchnapsen.Common.Commons;

namespace ThousandSchnapsen.Common.Interfaces
{
    public interface IAgent
    {
        Action GetAction(IPlayerState playerState);
    }
}