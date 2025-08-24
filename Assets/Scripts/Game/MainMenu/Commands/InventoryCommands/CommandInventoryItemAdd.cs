using MVVM.CMD;

namespace Game.MainMenu.Commands.InventoryCommands
{
    public class CommandInventoryItemAdd : ICommand
    {
        public int UniqueId;
        public int Amount;

        public CommandInventoryItemAdd(int uniqueId, int amount)
        {
            UniqueId = uniqueId;
            Amount = amount;
        }
    }
}