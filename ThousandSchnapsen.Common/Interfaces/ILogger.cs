using ThousandSchnapsen.Common.States;

namespace ThousandSchnapsen.Common.Interfaces
{
    public interface ILogger
    {
        void Log(GameState gameState);
    }
}