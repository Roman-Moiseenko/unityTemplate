using MVVM.CMD;

namespace Game.GamePlay.Commands.TowerCommand
{
    public class CommandTowerBust : ICommand
    {
        public string ConfigIdTower;
        public string ConfigIdBust;

        public CommandTowerBust(string configIdTower, string configIdBust)
        {
            ConfigIdTower = configIdTower;
            ConfigIdBust = configIdBust;
        }
    }
}