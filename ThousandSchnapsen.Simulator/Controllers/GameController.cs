using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ThousandSchnapsen.Common.Agents;
using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.Common.Interfaces;
using ThousandSchnapsen.Common.States;
using ThousandSchnapsen.Simulator.Dto;

namespace ThousandSchnapsen.Simulator.Controllers
{
    [ApiController]
    [Route("api")]
    public class GameController : ControllerBase
    {
        private const int DealerId = 0;

        private readonly ILogger<GameController> _logger;
        private IGameState _gameState;
        private int _playerId;
        private IAgent[] _opponents;

        public GameController(ILogger<GameController> logger) =>
            _logger = logger;

        [HttpPost("reset")]
        public ActionResult<IPlayerState> Reset(GameConfiguration gameConfiguration)
        {
            if (gameConfiguration.PlayerNo < 1 || gameConfiguration.PlayerNo > 3)
                return BadRequest("PlayerNo should be in range from 1 to 3");
            _playerId = gameConfiguration.PlayerNo;
            _gameState = new GameState(DealerId);
            _opponents = Enumerable
                .Range(0, Constants.PlayersCount)
                .Select(id => new FixedAgent(id))
                .ToArray();
            UpdateGameState();
            return Ok(_gameState.GetPlayerState(_playerId));
        }

        [HttpPost("step")]
        public ActionResult<StepActionResult> Step(Action action)
        {
            var (playerId, card) = action;
            if (playerId != _playerId)
                return BadRequest("Invalid Player ID");
            _gameState.PerformAction(action);
            UpdateGameState();
            return Ok(new StepActionResult()
            {
                Done = _gameState.GameFinished,
                PlayerState = _gameState.GetPlayerState(_playerId)
            });
        }

        private void UpdateGameState()
        {
            while (_gameState.NextPlayerId != _playerId && !_gameState.GameFinished)
                _gameState = _gameState.PerformAction(
                    _opponents[_gameState.NextPlayerId].GetAction(_gameState.GetNextPlayerState())
                );
        }
    }
}