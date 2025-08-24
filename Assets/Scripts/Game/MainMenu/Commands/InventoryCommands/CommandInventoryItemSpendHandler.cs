using System;
using System.Linq;
using Game.State.Inventory.TowerCards;
using Game.State.Root;
using MVVM.CMD;

namespace Game.MainMenu.Commands.InventoryCommands
{
    public class CommandInventoryItemSpendHandler : ICommandHandler<CommandInventoryItemSpend>
    {
        private readonly GameStateProxy _gameState;

        public CommandInventoryItemSpendHandler(GameStateProxy gameState)
        {
            _gameState = gameState;
        }


        public bool Handle(CommandInventoryItemSpend commandInventory)
        {
            var itemEntity = _gameState.Inventory.Items
                .FirstOrDefault(item => item.UniqueId == commandInventory.UniqueId);

            if (itemEntity == null)
            {
                throw new Exception("ItemEntity not Founded!");
            }


            if (itemEntity.Amount.Value < commandInventory.Amount)
            {
                throw new Exception("ItemEntity not Amount");
            }

            itemEntity.Amount.Value -= commandInventory.Amount;
            
            return true;
        }
    }
}