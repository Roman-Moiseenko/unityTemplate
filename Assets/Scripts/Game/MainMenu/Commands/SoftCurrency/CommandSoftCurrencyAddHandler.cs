using Game.State.Root;
using MVVM.CMD;

namespace Game.MainMenu.Commands.SoftCurrency
{
    public class CommandSoftCurrencyAddHandler : ICommandHandler<CommandSoftCurrencyAdd>
    {
        private readonly GameStateProxy _gameState;

        public CommandSoftCurrencyAddHandler(GameStateProxy gameState)
        {
            _gameState = gameState;
        }
        public bool Handle(CommandSoftCurrencyAdd command)
        {
            _gameState.SoftCurrency.Value += command.Value;
            return true;
        }
    }
}