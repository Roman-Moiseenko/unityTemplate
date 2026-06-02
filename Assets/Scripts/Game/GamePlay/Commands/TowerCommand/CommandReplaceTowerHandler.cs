using Game.GamePlay.View.Towers;
using Game.State.Gameplay;
using Game.State.Maps.Towers;
using Game.State.Root;
using MVVM.CMD;
using UnityEngine;

namespace Game.GamePlay.Commands.TowerCommand
{
    public class CommandReplaceTowerHandler : ICommandHandler<CommandReplaceTower>
    {
        private readonly GameplayStateProxy _gameplayState;

        public CommandReplaceTowerHandler(GameplayStateProxy gameplayState)
        {
            _gameplayState = gameplayState;
        }
        public bool Handle(CommandReplaceTower command)
        {
            TowerEntity firstTower = null;
            TowerEntity secondTower = null;
            foreach (var towerEntity in _gameplayState.Towers)
            {
                if (towerEntity.UniqueId == command.CardFirstFirstUniqueId) firstTower = towerEntity;
                if (towerEntity.UniqueId == command.CardSecondSecondUniqueId) secondTower = towerEntity;
            }

            if (firstTower == null || secondTower == null) return false;
            Debug.Log(firstTower.Position.CurrentValue);
            Debug.Log(secondTower.Position.CurrentValue);
            var pos = firstTower.Position.CurrentValue;
            firstTower.Position.Value = secondTower.Position.CurrentValue;
            secondTower.Position.Value = pos;
            return true;
        }
    }
}