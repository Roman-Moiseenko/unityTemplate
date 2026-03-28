using Game.State.Common;
using Game.State.Inventory;
using MVVM.CMD;

namespace Game.MainMenu.Commands.TowerCommands
{
    public class CommandTowerCardAdd : ICommand
    {
        public string ConfigId;
        public TypeEpic EpicLevel;
        public int Level;
    }
}