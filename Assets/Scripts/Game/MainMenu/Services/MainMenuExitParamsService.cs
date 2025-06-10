using DI;
using Game.GamePlay.Root;
using Game.MainMenu.Root;

namespace Game.MainMenu.Services
{
    public class MainMenuExitParamsService
    {
        private readonly DIContainer _container;

        public MainMenuExitParamsService(DIContainer container)
        {
            _container = container;
        }

        public MainMenuExitParams GetExitParams(int currentIdMap)
        {
            var gameplayEnterParams = new GameplayEnterParams(currentIdMap);
            gameplayEnterParams.DamageTowerBust = 1.5f;
            
            var mainMenuExitParams = new MainMenuExitParams(gameplayEnterParams);

            return mainMenuExitParams;
        }
    }
}