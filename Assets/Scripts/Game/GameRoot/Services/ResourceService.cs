using DI;
using Game.GameRoot.Commands.HardCurrency;
using MVVM.CMD;

namespace Game.GameRoot.Services
{
    public class ResourceService
    {
       // private readonly DIContainer _container;
        private readonly ICommandProcessor _cmd;

        public ResourceService(DIContainer container)
        {
         //   _container = container;
            _cmd = container.Resolve<ICommandProcessor>();
        }


        public bool SpendHardCurrency(int value)
        {
            var command = new CommandSpendHardCurrency(value);
            return _cmd.Process(command);
        }
    }
}