

using MVVM.CMD;

namespace Game.GamePlay.Commands.TowerCommand
{
    public class CommandDeleteTower : ICommand
    {
        public readonly int UniqueId;

        public CommandDeleteTower(int uniqueId)
        {
            UniqueId = uniqueId;
        }
    }
}