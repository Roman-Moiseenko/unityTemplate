using System;
using System.Linq;
using Game.State.Root;
using MVVM.CMD;
using UnityEngine;

namespace Game.GamePlay.Commands.WarriorCommands
{
    public class CommandRemoveWarriorTowerHandler : ICommandHandler<CommandRemoveWarriorTower>
    {
        private readonly GameplayStateProxy _gameplayState;

        public CommandRemoveWarriorTowerHandler(GameplayStateProxy gameplayState)
        {
            _gameplayState = gameplayState;
        }
        public bool Handle(CommandRemoveWarriorTower command)
        {
            foreach (var warriorEntity in _gameplayState.Warriors.ToList())
            {
                if (warriorEntity.UniqueId == command.UniqueId)
                {
                    _gameplayState.Warriors.Remove(warriorEntity);
                    return false;
                }
            }
            throw new Exception("Ошибка, warrior не найден" + command.UniqueId);

        }
    }
}