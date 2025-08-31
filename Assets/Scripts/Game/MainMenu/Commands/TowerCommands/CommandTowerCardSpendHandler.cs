using Game.State.Root;
using MVVM.CMD;

namespace Game.MainMenu.Commands.TowerCommands
{
    public class CommandTowerCardSpendHandler : ICommandHandler<CommandTowerCardSpend>
    {
        private readonly GameStateProxy _gameState;

        public CommandTowerCardSpendHandler(GameStateProxy gameState)
        {
            _gameState = gameState;
        }
        
        public bool Handle(CommandTowerCardSpend command)
        {
            _gameState.Inventory.RemoveItem(command.UniqueId);
            return true;
        }
    }
}