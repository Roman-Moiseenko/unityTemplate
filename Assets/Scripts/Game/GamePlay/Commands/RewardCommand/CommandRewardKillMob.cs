using Game.State.Inventory;
using MVVM.CMD;

namespace Game.GamePlay.Commands.RewardCommand
{
    public class CommandRewardKillMob : ICommand
    {
        public readonly int SoftCurrency;
        public readonly int Progress;
        public readonly InventoryItemData InventoryItemData;

        public CommandRewardKillMob(int softCurrency, int progress, InventoryItemData inventoryItemData = null)
        {
            SoftCurrency = softCurrency;
            Progress = progress;
            InventoryItemData = inventoryItemData;
        }
    }
}