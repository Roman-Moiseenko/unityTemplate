using Game.State.Inventory;
using Game.State.Root;
using MVVM.CMD;

namespace Game.GamePlay.Commands.RewardCommand
{
    public class CommandRewardKillMobHandler : ICommandHandler<CommandRewardKillMob>
    {
        public const int CoefRewardProgress = 50;
        private readonly GameplayStateProxy _gameplayState;

        public CommandRewardKillMobHandler(GameplayStateProxy gameplayState)
        {
            _gameplayState = gameplayState;
        }
        public bool Handle(CommandRewardKillMob command)
        {
            _gameplayState.SoftCurrency.Value += command.SoftCurrency;
            _gameplayState.Progress.Value += command.Progress * CoefRewardProgress;
//            _gameplayState.Progress.Value += command.Progress * CoefRewardProgress / (_gameplayState.ProgressLevel.CurrentValue + 1);
            
            //if (command.InventoryData != null) _gameplayState.Inventory.Add(InventoryFactory.CreateInventory(command.InventoryData));

            return true;

        }
    }
}