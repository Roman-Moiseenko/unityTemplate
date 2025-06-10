using Game.State.CMD;
using Game.State.Inventory;

namespace Game.GamePlay.Commands.RewardCommand
{
    public class CommandRewardKillMob : ICommand
    {
        public readonly int SoftCurrency;
        public readonly int Progress;
        public readonly InventoryData InventoryData;

        public CommandRewardKillMob(int softCurrency, int progress, InventoryData inventoryData = null)
        {
            SoftCurrency = softCurrency;
            Progress = progress;
            InventoryData = inventoryData;
        }
    }
}