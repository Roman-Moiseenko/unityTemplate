using Game.State.Inventory.TowerPlans;
using Game.State.Root;
using MVVM.CMD;
using UnityEngine;

namespace Game.MainMenu.Commands.TowerCommands
{
    public class CommandTowerPlanAddHandler : ICommandHandler<CommandTowerPlanAdd>
    {
        private readonly GameStateProxy _gameState;

        public CommandTowerPlanAddHandler(GameStateProxy gameState)
        {
            _gameState = gameState;
        }

        public bool Handle(CommandTowerPlanAdd command)
        {
            var towerPlanData = new TowerPlanData
            {
                UniqueId = _gameState.CreateInventoryID(),
                ConfigId = command.ConfigId,
                Amount = command.Amount,
            };
            
            _gameState.Inventory.AddItem(towerPlanData);
            return true;
        }
    }
}