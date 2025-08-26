using MVVM.CMD;

namespace Game.MainMenu.Commands.TowerCommands
{
    public class CommandTowerPlanSpend : ICommand
    {
        public string ConfigId;
        public int Amount;
    }
}