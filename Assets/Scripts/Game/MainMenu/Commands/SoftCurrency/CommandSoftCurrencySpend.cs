using MVVM.CMD;

namespace Game.MainMenu.Commands.SoftCurrency
{
    public class CommandSoftCurrencySpend : ICommand
    {
        public int Value;

        public CommandSoftCurrencySpend(int value)
        {
            Value = value;
        }
    }
}