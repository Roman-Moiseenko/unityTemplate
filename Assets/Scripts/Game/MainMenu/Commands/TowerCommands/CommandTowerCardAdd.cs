using Game.State.Inventory;
using MVVM.CMD;

namespace Game.MainMenu.Commands.TowerCommands
{
    public class CommandTowerCardAdd : ICommand
    {
        public string ConfigId;
        public TypeEpicCard EpicLevel;
        public int Level;
    }
}