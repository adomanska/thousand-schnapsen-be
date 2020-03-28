﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ThousandSchnapsen.Common.Agents;
using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.Common.Interfaces;
using ThousandSchnapsen.Common.States;
using ThousandSchnapsen.Simulator.Dto;
using Action = ThousandSchnapsen.Common.Commons.Action;

namespace ThousandSchnapsen.Simulator.Controllers
{
    public static class CacheKeys
    {
        public static string GameState => "_GameState";
        public static string Opponents => "_Opponents";
        public static string PlayerId => "_PlayerId";
    }

    [ApiController]
    [Route("api/game")]
    public class GameController : ControllerBase
    {
        private const int DealerId = 0;

        private readonly IMemoryCache _cache;
        private readonly IMapper _mapper;

        public GameController(IMemoryCache cache, IMapper mapper)
        {
            _cache = cache;
            _mapper = mapper;
        }

        [HttpPost("reset")]
        public ActionResult<PlayerState> Reset(GameConfigurationDto gameConfiguration)
        {
            if (gameConfiguration.PlayerNo < 1 || gameConfiguration.PlayerNo > 3)
                return BadRequest("PlayerNo should be in range from 1 to 3");

            var opponents = Enumerable
                .Range(0, Constants.PlayersCount)
                .Select(id => new FixedAgent(id))
                .ToArray();
            var playerId = gameConfiguration.PlayerNo;
            var gameState = UpdateGameState(new GameState(DealerId), opponents, playerId);

            _cache.Set(CacheKeys.GameState, gameState);
            _cache.Set(CacheKeys.Opponents, opponents);
            _cache.Set(CacheKeys.PlayerId, playerId);

            return Ok(GetActionResult(gameState, playerId));
        }

        [HttpPost("step")]
        public ActionResult<ActionResultDto> Step(ActionDto action)
        {
            if (_cache.TryGetValue(CacheKeys.PlayerId, out int playerId) &&
                _cache.TryGetValue(CacheKeys.Opponents, out IAgent[] opponents) &&
                _cache.TryGetValue(CacheKeys.GameState, out GameState gameState))
            {
                if (action.PlayerId != playerId)
                    return BadRequest("Invalid Player ID");
                try
                {
                    gameState = gameState.PerformAction(new Action()
                    {
                        PlayerId = action.PlayerId,
                        Card = new Card(action.CardId)
                    });
                    gameState = UpdateGameState(
                        gameState,
                        opponents,
                        playerId
                    );
                    _cache.Set(CacheKeys.GameState, gameState);
                }
                catch
                {
                    return BadRequest("Invalid action");
                }
                return Ok(GetActionResult(gameState, playerId));
            }

            throw new Exception("Caching problem occured");
        }

        private GameState UpdateGameState(GameState gameState, IReadOnlyList<IAgent> opponents, int playerId)
        {
            while (gameState.NextPlayerId != playerId && !gameState.GameFinished)
                gameState = gameState.PerformAction(
                    opponents[gameState.NextPlayerId].GetAction(gameState.GetNextPlayerState())
                );
            return gameState;
        }

        private ActionResultDto GetActionResult(GameState gameState, int playerId) =>
            new ActionResultDto()
            {
                Done = gameState.GameFinished,
                State = _mapper.Map<PlayerStateDto>(gameState.GetPlayerState(playerId)),
                Info = new InfoDto()
                {
                    AvailableCardsIds = gameState.NextPlayerId == playerId && !gameState.GameFinished
                        ? gameState
                            .GetAvailableActions()
                            .Select(availableAction => availableAction.Card.CardId)
                            .ToArray()
                        : null
                }
            };
    }
}