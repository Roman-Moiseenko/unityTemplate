using Game.State.Maps.Towers;
using Game.State.Root;
using MVVM.CMD;
using UnityEngine;

namespace Game.GamePlay.Commands.TowerCommand
{
    public class CommandMoveTowerHandler : ICommandHandler<CommandMoveTower>
    {
        private readonly GameplayStateProxy _gameplayState;

        public CommandMoveTowerHandler(GameplayStateProxy gameplayState)
        {
            _gameplayState = gameplayState;
        }
        public bool Handle(CommandMoveTower command)
        {
            foreach (var entity in _gameplayState.Towers)
            {
                if (entity  is TowerEntity towerEntity && towerEntity.UniqueId == command.UniqueId)
                {
                    towerEntity.Position.Value = command.Position;
                    return false;
                }
            }
            return false; //Не сохраняем
        }
    }
}