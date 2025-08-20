using MVVM.CMD;

namespace Game.GameRoot.Commands.HardCurrency
{
    public class CommandSpendHardCurrency : ICommand
    {
        public int Value;

        public CommandSpendHardCurrency(int value)
        {
            Value = value;
        }
    }
}