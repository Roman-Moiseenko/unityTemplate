using MVVM.CMD;

namespace Game.MainMenu.Commands.TowerCommands
{
    public class CommandTowerPlanAdd : ICommand
    {
        public string ConfigId;
        public int Amount;
    }
}