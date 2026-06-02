using MVVM.CMD;

namespace Game.GamePlay.Commands.TowerCommand
{
    public class CommandReplaceTower : ICommand
    {
        public readonly int CardFirstFirstUniqueId;
        public readonly int CardSecondSecondUniqueId;

        public CommandReplaceTower(int cardFirstUniqueId, int cardSecondUniqueId)
        {
            CardFirstFirstUniqueId = cardFirstUniqueId;
            CardSecondSecondUniqueId = cardSecondUniqueId;
        }
    }
}