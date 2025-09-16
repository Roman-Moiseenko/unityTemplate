using System;
using Game.State.Inventory.Chests;
using Game.State.Root;
using MVVM.CMD;

namespace Game.MainMenu.Commands.ChestCommands
{
    public class CommandChestOpeningHandler : ICommandHandler<CommandChestOpening>
    {
        private GameStateProxy _gameState;

        public CommandChestOpeningHandler(GameStateProxy gameState)
        {
            _gameState = gameState;
        }
        public bool Handle(CommandChestOpening command)
        {
            if (_gameState.ContainerChests.CellOpening.CurrentValue != 0) return false;
            
            _gameState.ContainerChests.CellOpening.Value = command.Cell;
            _gameState.ContainerChests.Chests[command.Cell].Status.Value = StatusChest.Opening;
            //_gameState.ContainerChests.StartOpening.OnNext(DateTime.Now.ToUniversalTime().ToFileTimeUtc());
            
            return true;
        }
    }
}