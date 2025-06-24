using DI;
using Game.GamePlay.Root;
using Game.MainMenu.Root;
using Game.State;
using Game.State.Root;

namespace Game.MainMenu.Services
{
    public class MainMenuExitParamsService
    {
        private readonly DIContainer _container;
        private readonly GameStateProxy _gameState;

        public MainMenuExitParamsService(DIContainer container)
        {
            _container = container;
            _gameState = container.Resolve<IGameStateProvider>().GameState;
        }

        public MainMenuExitParams GetExitParams(int currentIdMap)
        {
            var gameplayEnterParams = new GameplayEnterParams(currentIdMap);
            gameplayEnterParams.DamageTowerBust = 1.5f;
            //TODO Передаем сохраненные настройки геймплея 
            gameplayEnterParams.GameSpeed = _gameState.GameSpeed.CurrentValue;
            
            var mainMenuExitParams = new MainMenuExitParams(gameplayEnterParams);

            return mainMenuExitParams;
        }
    }
}