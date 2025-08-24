using MVVM.CMD;

namespace Game.MainMenu.Commands.TowerCommands
{
    public class CommandTowerCardLevelUp : ICommand
    {
        public int UniqueId;

        public CommandTowerCardLevelUp(int uniqueId)
        {
            UniqueId = uniqueId;
        }
    }
}