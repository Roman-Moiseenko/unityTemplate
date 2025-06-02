using System;
using System.Linq;
using Game.State.CMD;
using Game.State.Root;
using UnityEngine;

namespace Game.GamePlay.Commands.GroundCommands
{
    public class CommandCreateGroundHandler : ICommandHandler<CommandCreateGround>
    {
        private readonly GameStateProxy _gameState;

        public CommandCreateGroundHandler(GameStateProxy gameState)
        {
            _gameState = gameState;
        }
        public bool Handle(CommandCreateGround command)
        {
            var currentMap = _gameState.Maps.FirstOrDefault(m => m.Id == _gameState.CurrentMapId.CurrentValue);
            if (currentMap == null)
            {
                Debug.Log($" Карта не найдена { _gameState.CurrentMapId.CurrentValue}");
                return false;
            }

            throw new Exception("***");
        }
    }
}