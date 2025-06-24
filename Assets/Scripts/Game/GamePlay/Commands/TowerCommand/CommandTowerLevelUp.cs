using MVVM.CMD;

namespace Game.GamePlay.Commands.TowerCommand
{
    public class CommandTowerLevelUp : ICommand
    {
        public string ConfigId;

        public CommandTowerLevelUp(string configId)
        {
            ConfigId = configId;
            
        }
    }
}