using System;
using System.Linq;
using System.Text;
using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.Common.Interfaces;
using ThousandSchnapsen.Common.States;

namespace ThousandSchnapsen.Common.Loggers
{
    public class Logger : ILogger
    {
        public Logger() =>
            Console.OutputEncoding = Encoding.UTF8;

        public void Log(GameState gameState)
        {
            Console.WriteLine(Utils.CreateTitle("GAME STATE", 42));
            LogStock(gameState);
            LogResults(gameState);
            LogTrump(gameState);
            LogPlayersCards(gameState);
        }

        private static void LogStock(GameState gameState)
        {
            Console.WriteLine("STOCK:");
            gameState.Stock
                .Select(stockItem => $"{stockItem.PlayerId}: {stockItem.Card}")
                .ToList()
                .ForEach(Console.WriteLine);
            Console.WriteLine();
        }

        private static void LogResults(GameState gameState)
        {
            Console.WriteLine("RESULTS:");
            Console.WriteLine(
                string.Join('|', Enumerable
                    .Range(1, Constants.PlayersCount)
                    .Select(id => $"{id,4}")
                )
            );
            Console.WriteLine(
                string.Join('|', gameState.PlayersPoints
                    .Select(id => $"{id,4}")
                )
            );
            Console.WriteLine();
        }

        private static void LogTrump(GameState gameState)
        {
            Console.WriteLine($"TRUMP: {gameState.Trump}");
            Console.WriteLine();
        }

        private void LogPlayersCards(GameState gameState)
        {
            Console.WriteLine("PLAYERS CARDS:");

            string PlayerSymbol(int playerId) =>
                playerId == gameState.DealerId
                    ? "(D)"
                    : (playerId == gameState.NextPlayerId ? " ->" : "   ");

            Enumerable.Range(0, Constants.PlayersCount)
                .Select(playerId => $"{PlayerSymbol(playerId)} {playerId}:  {gameState.PlayersCards[playerId]}")
                .ToList()
                .ForEach(Console.WriteLine);
            Console.WriteLine();
        }
    }
}