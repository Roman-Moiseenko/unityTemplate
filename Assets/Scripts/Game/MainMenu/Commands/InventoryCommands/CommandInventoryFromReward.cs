using Game.State.Inventory;
using MVVM.CMD;

namespace Game.MainMenu.Commands.InventoryCommands
{
    public class CommandInventoryFromReward : ICommand
    {
        public InventoryType InventoryType;
        public string ConfigId;
        public long Amount;
    }
}