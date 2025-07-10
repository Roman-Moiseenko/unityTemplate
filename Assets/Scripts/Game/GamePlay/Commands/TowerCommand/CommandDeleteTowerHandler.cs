using Game.State.Maps.Towers;
using Game.State.Root;
using MVVM.CMD;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.GamePlay.Commands.TowerCommand
{
    public class CommandDeleteTowerHandler : ICommandHandler<CommandDeleteTower>
    {
        private readonly GameplayStateProxy _gameplayState;

        public CommandDeleteTowerHandler(GameplayStateProxy gameplayState)
        {
            _gameplayState = gameplayState;
        }
        
        public bool Handle(CommandDeleteTower command)
        {
            foreach (var entity in _gameplayState.Towers)
            {
                if (entity  is TowerEntity towerEntity && towerEntity.UniqueId == command.UniqueId)
                {
                    _gameplayState.Towers.Remove(towerEntity);
                    return true;
                }
            }

            return false;
        }
    }
}