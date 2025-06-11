using Game.Settings;
using Game.Settings.Gameplay.Entities.Busts;
using Game.State.Maps.Towers;
using Game.State.Root;
using MVVM.CMD;
using UnityEngine;

namespace Game.GamePlay.Commands.TowerCommand
{
    public class CommandTowerBustHandler : ICommandHandler<CommandTowerBust>
    {
        private readonly GameplayStateProxy _gameplayState;
        private readonly TowerBustsSettings _bustsSettings;

        public CommandTowerBustHandler(GameplayStateProxy gameplayState, GameSettings gameSettings)
        {
            _gameplayState = gameplayState;
            _bustsSettings = gameSettings.TowerBustsSettings;
        }
        public bool Handle(CommandTowerBust command)
        {
            //Загружаем настройки бустеров
            foreach (var bust in _bustsSettings.Busts)
            {
                if (bust.ConfigId == command.ConfigIdBust)
                {                
                    ApplyBustTower(command.ConfigIdTower, bust);
                    return true;
                }
            }
            return false;
        }

        private void ApplyBustTower(string configIdTower, TowerBustSettings bustSettings)
        {
            foreach (var entity in _gameplayState.Entities)
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