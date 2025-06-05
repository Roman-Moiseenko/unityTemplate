using DI;

namespace Game.GamePlay.Services
{
    public class InputController
    {
        private readonly DIContainer _container;

        public InputController(DIContainer container)
        {
            _container = container;
        }
    }
}