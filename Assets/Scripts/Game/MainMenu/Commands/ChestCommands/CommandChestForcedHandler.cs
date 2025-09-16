using System;
using Game.State.Root;
using MVVM.CMD;

namespace Game.MainMenu.Commands.ChestCommands
{
    public class CommandChestForcedHandler : ICommandHandler<CommandChestForced>
    {
        private GameStateProxy _gameState;

        public CommandChestForcedHandler(GameStateProxy gameState)
        {
            _gameState = gameState;
        }

        public bool Handle(CommandChestForced command)
        {
            var start = DateTime.FromFileTimeUtc(_gameState.ContainerChests.StartOpening.CurrentValue);
            start -= TimeSpan.FromHours(1);

            _gameState.ContainerChests.StartOpening.Value = start.ToFileTimeUtc();
            return true;
        }
    }
}