using System;
using System.Linq;
using Game.State.Root;
using MVVM.CMD;

namespace Game.MainMenu.Commands.InventoryCommands
{
    public class CommandInventoryItemAddHandler : ICommandHandler<CommandInventoryItemAdd>
    {
        private readonly GameStateProxy _gameState;

        public CommandInventoryItemAddHandler(GameStateProxy gameState)
        {
            _gameState = gameState;
        }


        public bool Handle(CommandInventoryItemAdd commandInventory)
        {
            var itemEntity = _gameState.Inventory.Items
                .FirstOrDefault(item => item.UniqueId == commandInventory.UniqueId);

            if (itemEntity == null)
            {
                throw new Exception("ItemEntity not Founded!");
            }
            

            itemEntity.Amount.Value += commandInventory.Amount;
            
            return true;
        }
    }
}