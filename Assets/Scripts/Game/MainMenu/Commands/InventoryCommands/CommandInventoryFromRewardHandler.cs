using Game.MainMenu.Commands.SoftCurrency;
using Game.MainMenu.Commands.TowerCommands;
using Game.State.Inventory;
using Game.State.Root;
using MVVM.CMD;

namespace Game.MainMenu.Commands.InventoryCommands
{
    public class CommandInventoryFromRewardHandler : ICommandHandler<CommandInventoryFromReward>
    {
        private readonly ICommandProcessor _cmd;

        public CommandInventoryFromRewardHandler(ICommandProcessor cmd)
        {
            _cmd = cmd;
        }
        
        /**
         * Добавление в инвентарь награды по типу InventoryType
         */
        public bool Handle(CommandInventoryFromReward command)
        {
            switch (command.InventoryType)
            {
                case InventoryType.Other when command.ConfigId == "Currency":
                {
                    var commandReward = new CommandSoftCurrencyAdd()
                    {
                        Value = command.Amount
                    };

                    _cmd.Process(commandReward);
                    return true;
                }
                case InventoryType.TowerPlan:
                {
                    var commandReward = new CommandTowerPlanAdd()
                    {
                        ConfigId = command.ConfigId,
                        Amount = command.Amount
                    };

                    _cmd.Process(commandReward);
                    return true;
                }
                case InventoryType.TowerCard:
                {
                    var commandReward = new CommandTowerCardAdd()
                    {
                        ConfigId = command.ConfigId,
                    };

                    _cmd.Process(commandReward);
                    return true;
                }
                case InventoryType.SkillCard:
                case InventoryType.SkillPlan:
                case InventoryType.HeroCard:
                default:
                    return false;
            }
        }
    }
}