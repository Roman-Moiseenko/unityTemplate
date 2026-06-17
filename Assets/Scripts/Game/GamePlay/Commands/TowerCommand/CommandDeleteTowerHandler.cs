using Game.State.Gameplay;
using Game.State.Maps.Towers;
using MVVM.CMD;

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
                    _gameplayState.StatisticGame.DestroyTower();
                    return true;
                }
            }

            return false;
        }
    }
}