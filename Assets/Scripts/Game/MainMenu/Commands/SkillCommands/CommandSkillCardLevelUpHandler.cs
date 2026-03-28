using System;
using System.Linq;
using Game.State.Inventory.SkillCards;
using Game.State.Root;
using MVVM.CMD;

namespace Game.MainMenu.Commands.SkillCommands
{
    public class CommandSkillCardLevelUpHandler : ICommandHandler<CommandSkillCardLevelUp>
    {
        private readonly GameStateProxy _gameState;

        public CommandSkillCardLevelUpHandler(GameStateProxy gameState)
        {
            _gameState = gameState;
        }
        public bool Handle(CommandSkillCardLevelUp command)
        {
            
            var itemEntity = _gameState.Inventory.Items
                .FirstOrDefault(item => item.UniqueId == command.UniqueId);

            if (itemEntity == null)
            {
                throw new Exception("ItemEntity not Founded!");
            }

            var skillCardEntity = itemEntity.As<SkillCard>();
            skillCardEntity.Level.Value++;
            
            return true;
        }
    }
}