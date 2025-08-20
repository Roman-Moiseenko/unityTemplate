using Game.State.Root;
using MVVM.CMD;

namespace Game.GameRoot.Commands.HardCurrency
{
    public class CommandSpendHardCurrencyHandler : ICommandHandler<CommandSpendHardCurrency>
    {
        private readonly GameStateProxy _gameState;

        public CommandSpendHardCurrencyHandler(GameStateProxy gameState)
        {
            _gameState = gameState;
        }

        public bool Handle(CommandSpendHardCurrency command)
        {
            if (_gameState.HardCurrency.CurrentValue < command.Value) return false;
            //TODO Проверка, если не хватает вызвать блок рекламы
            _gameState.HardCurrency.Value -= command.Value;
            
            return true;
            
        }
    }

    


}