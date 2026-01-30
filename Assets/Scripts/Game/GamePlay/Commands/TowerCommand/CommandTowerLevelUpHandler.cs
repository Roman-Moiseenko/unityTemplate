using Game.Settings;
using Game.State.Maps.Towers;
using Game.State.Root;
using MVVM.CMD;
using UnityEngine;

namespace Game.GamePlay.Commands.TowerCommand
{
    public class CommandTowerLevelUpHandler : ICommandHandler<CommandTowerLevelUp>
    {
        private readonly GameplayStateProxy _gameplayState;


        public CommandTowerLevelUpHandler(GameplayStateProxy gameplayState, GameSettings gameSettings)
        {
            _gameplayState = gameplayState;
        }

        public bool Handle(CommandTowerLevelUp command)
        {
            foreach (var entity in _gameplayState.Towers)
            {
                if (entity.ConfigId == command.ConfigId)
                {

                    entity.Level.Value += 1;
                }
            }

            return true; //Сохраняем результат
        }
    }
}