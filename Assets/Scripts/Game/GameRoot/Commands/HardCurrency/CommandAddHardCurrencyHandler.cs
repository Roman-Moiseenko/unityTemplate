using Game.State.Root;
using MVVM.CMD;
using UnityEngine;

namespace Game.GameRoot.Commands.HardCurrency
{
    public class CommandAddHardCurrencyHandler : ICommandHandler<CommandAddHardCurrency>
    {
        private readonly GameStateProxy _gameState;

        public CommandAddHardCurrencyHandler(GameStateProxy gameState)
        {
            _gameState = gameState;
        }

        public bool Handle(CommandAddHardCurrency command)
        {
            _gameState.HardCurrency.Value += command.Value;
            return true;
        }
    }

    


}