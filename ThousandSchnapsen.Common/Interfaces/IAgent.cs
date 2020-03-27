using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.Common.States;

namespace ThousandSchnapsen.Common.Interfaces
{
    public interface IAgent
    {
        Action GetAction(PlayerState playerState);
    }
}