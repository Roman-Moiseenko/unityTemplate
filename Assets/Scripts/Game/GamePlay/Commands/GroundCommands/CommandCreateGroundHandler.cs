using System;
using System.Linq;
using Game.State.Root;
using MVVM.CMD;
using UnityEngine;

namespace Game.GamePlay.Commands.GroundCommands
{
    public class CommandCreateGroundHandler : ICommandHandler<CommandCreateGround>
    {
        private readonly GameplayStateProxy _gameplayState;

        public CommandCreateGroundHandler(GameplayStateProxy gameplayState)
        {
            _gameplayState = gameplayState;
        }
        public bool Handle(CommandCreateGround command)
        {
            /*
            var currentMap = _gameplayState.Maps.FirstOrDefault(m => m.Id == _gameplayState.CurrentMapId.CurrentValue);
            if (currentMap == null)
            {
                Debug.Log($" Карта не найдена { _gameplayState.CurrentMapId.CurrentValue}");
                return false;
            }
*/
            throw new Exception("***");
        }
    }
}