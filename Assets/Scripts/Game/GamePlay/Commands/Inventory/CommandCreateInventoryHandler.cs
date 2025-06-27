using Game.Settings;
using Game.State.Root;
using JetBrains.Annotations;
using MVVM.CMD;

namespace Game.GamePlay.Commands.Inventory
{
    public class CommandCreateInventoryHandler : ICommandHandler<CommandCreateInventory>
    {
        private readonly GameStateProxy _gameState;
        private readonly GameSettings _gameSettings;

        public CommandCreateInventoryHandler(GameStateProxy gameState, GameSettings gameSettings)
        {
            _gameState = gameState;
            _gameSettings = gameSettings;
        }
        
        public bool Handle(CommandCreateInventory commandCreate)
        {

            return true;
        }
    }
}