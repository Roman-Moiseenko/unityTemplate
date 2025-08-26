using Game.State.Inventory;
using Game.State.Inventory.TowerPlans;
using Game.State.Root;
using MVVM.CMD;

namespace Game.MainMenu.Commands.TowerCommands
{
    public class CommandTowerPlanSpendHandler : ICommandHandler<CommandTowerPlanSpend>
    {
        private readonly GameStateProxy _gameState;

        public CommandTowerPlanSpendHandler(GameStateProxy gameState)
        {
            _gameState = gameState;
        }

        public bool Handle(CommandTowerPlanSpend command)
        {
            var towerPlan = _gameState.Inventory.GetByConfigAndType<TowerPlan>(InventoryType.TowerPlan, command.ConfigId);
            if (towerPlan.Amount.Value < command.Amount) return false;
            if (towerPlan.Amount.Value == command.Amount)
            {
                _gameState.Inventory.RemoveItem(towerPlan);
            }
            else
            {
                towerPlan.Amount.Value -= command.Amount;
            }
            
            return true;
        }
    }
}