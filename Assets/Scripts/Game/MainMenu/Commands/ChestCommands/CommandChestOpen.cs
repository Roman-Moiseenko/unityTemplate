using System.Collections.Generic;
using Game.State.Inventory;
using Game.State.Maps.Rewards;
using MVVM.CMD;

namespace Game.MainMenu.Commands.ChestCommands
{
    public class CommandChestOpen : ICommand
    {
        public List<RewardEntityData> Rewards { get; set; }
        public int Cell { get; set; }
    }
}