using MVVM.CMD;

namespace Game.MainMenu.Commands.InventoryCommands
{
    public class CommandInventoryItemSpend : ICommand
    {
        public int UniqueId;
        public int Amount;

        public CommandInventoryItemSpend(int uniqueId, int amount)
        {
            UniqueId = uniqueId;
            Amount = amount;
        }  
    }
}