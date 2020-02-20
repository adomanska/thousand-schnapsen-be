using System.Linq;
using System;
using System.Text;

namespace ThousandSchnapsen.Common
{
    public class Logger: ILogger
    {
        public Logger() =>
            Console.OutputEncoding = Encoding.UTF8;
        public void Log(IGameState gameState)
        {
            Console.WriteLine(Utils.CreateTitle("GAME STATE", 42));
            LogStock(gameState);
            LogResults(gameState);
            LogTrump(gameState);
            LogPlayersCards(gameState);
        }

        private void LogStock(IGameState gameState)
        {
            Console.WriteLine("STOCK:");
            gameState.Stock
                .Select(stockItem => $"{stockItem.PlayerId + 1}: {stockItem.Card}")
                .ToList()
                .ForEach(stockItemString => Console.WriteLine(stockItemString));
        }

        private void LogResults(IGameState gameState)
        {
            Console.WriteLine("RESULTS:");
            Console.WriteLine(
                String.Join('|', Enumerable
                    .Range(1, Constants.PLAYERS_COUNT)
                    .Select(id => String.Format("{0,4}", id))
                )
            );
            Console.WriteLine(
                String.Join('|', gameState.PlayersPoints
                    .Select(id => String.Format("{0,4}", id))
                )
            );
        }

        private void LogTrump(IGameState gameState)
        {
            Console.WriteLine($"TRUMP: {gameState.Trump}");
        }

        private void LogPlayersCards(IGameState gameState)
        {
            Console.WriteLine("PLAYERS CARDS:");
            Func<int, string> playerSymbol = playerId => 
                playerId == gameState.DealerId 
                    ? "(D)" 
                    : (playerId == gameState.NextPlayerId ? " ->" : "   ");
            Enumerable.Range(0, Constants.PLAYERS_COUNT)
                .Select(playerId => $"{playerSymbol(playerId)} {playerId + 1}:  {gameState.PlayersCards[playerId]}")
                .ToList()
                .ForEach(playerCardsString => Console.WriteLine(playerCardsString));
        }
    }
}