using Game.State.Root;
using MVVM.CMD;

namespace Game.MainMenu.Commands.SoftCurrency
{
    public class CommandSoftCurrencySpendHandler : ICommandHandler<CommandSoftCurrencySpend>
    {
        private readonly GameStateProxy _gameState;

        public CommandSoftCurrencySpendHandler(GameStateProxy gameState)
        {
            _gameState = gameState;
        }

        public bool Handle(CommandSoftCurrencySpend command)
        {
            if (_gameState.SoftCurrency.CurrentValue < command.Value) return false;
            //TODO Проверка, если не хватает вызвать блок обмена за кристаллы
            _gameState.SoftCurrency.Value -= command.Value;
            
            return true;
            
        }
    }
}