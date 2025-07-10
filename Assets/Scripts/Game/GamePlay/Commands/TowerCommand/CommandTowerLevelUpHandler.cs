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
            //Загружаем настройки уровней башен
            ApplyLevelUpTower(command.ConfigId);

            return true;
        }

        private void ApplyLevelUpTower(string configIdTower)
        {
            foreach (var entity in _gameplayState.Towers)
            {
                if (entity is TowerEntity towerEntity && towerEntity.ConfigId == configIdTower)
                {
                    //TODO Применить навыки к башням
                    // entity.Damage += bustSettings.Damage;
                    entity.Level.Value += 1;
                }
            }
        }
    }
}