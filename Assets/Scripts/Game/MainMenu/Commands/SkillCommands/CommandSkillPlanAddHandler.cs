using Game.State.Inventory.SkillPlans;
using Game.State.Root;
using MVVM.CMD;

namespace Game.MainMenu.Commands.SkillCommands
{
    public class CommandSkillPlanAddHandler : ICommandHandler<CommandSkillPlanAdd>
    {
        private readonly GameStateProxy _gameState;

        public CommandSkillPlanAddHandler(GameStateProxy gameState)
        {
            _gameState = gameState;
        }

        public bool Handle(CommandSkillPlanAdd command)
        {
            var skillPlanData = new SkillPlanData
            {
                UniqueId = _gameState.CreateInventoryID(),
                ConfigId = command.ConfigId,
                Amount = command.Amount,
            };
            
            _gameState.Inventory.AddItem(skillPlanData);
            return true;
        }
    }
}