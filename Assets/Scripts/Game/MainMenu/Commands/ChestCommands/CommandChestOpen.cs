using System.Collections.Generic;
using Game.State.Inventory;
using MVVM.CMD;

namespace Game.MainMenu.Commands.ChestCommands
{
    public class CommandChestOpen : ICommand
    {
        public Dictionary<InventoryType, Dictionary<string, int>> Rewards { get; set; }
        public int Cell { get; set; }
    }
}