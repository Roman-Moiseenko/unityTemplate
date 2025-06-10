using DI;
using Game.GamePlay.Root;
using Game.MainMenu.Root;
using Game.State;
using Game.State.Root;
using R3;
using UnityEngine;

namespace Game.GamePlay.Services
{
    /**
     * Сервис для подписок на события которые необходимо сохранить.
     */
    public class GameplayService
    {
        private readonly Subject<GameplayExitParams> _exitSceneRequest;
        private readonly DIContainer _container;
        private readonly GameplayStateProxy _gameplayState;

        public GameplayService(Subject<GameplayExitParams> exitSceneRequest, DIContainer container)
        {
            _exitSceneRequest = exitSceneRequest;
            _container = container;
            _gameplayState = container.Resolve<IGameStateProvider>().GameplayState;
            //TODO Подписка на последнюю волну и мобов, как закончатся, то Win()

            //TODO Подписка на здоровье крепости, как закончиться, то Lose()
        }

        public void Win() //private
        {
            var menuParams = new MainMenuEnterParams("Выход");
            //Передаем награду и некоторые настройки, для загрузки в меню
            menuParams.GameSpeed = _gameplayState.PreviousGameSpeed;
            menuParams.SoftCurrency = _gameplayState.SoftCurrency.CurrentValue;
            menuParams.CompletedLevel = true;
            var exitParams = new GameplayExitParams(menuParams);
            _exitSceneRequest.OnNext(exitParams); 
        }

        public void Lose()  //private
        {
            var menuParams = new MainMenuEnterParams("Выход");
            //Передаем награду и некоторые настройки, для загрузки в меню
            menuParams.GameSpeed = _gameplayState.PreviousGameSpeed;
            menuParams.SoftCurrency = _gameplayState.SoftCurrency.CurrentValue;
            menuParams.LastWave = _gameplayState.CurrentWave.CurrentValue - 1; //Текущая волна -1
            menuParams.CompletedLevel = false; //Не завершен
            var exitParams = new GameplayExitParams(menuParams);
            _exitSceneRequest.OnNext(exitParams);
        }

        /**
         * Прервать игру
         */
        public void Abort()
        {
            var menuParams = new MainMenuEnterParams("Выход");
            //Передаем награду и некоторые настройки, для загрузки в меню
            menuParams.GameSpeed = _gameplayState.PreviousGameSpeed;
            menuParams.SoftCurrency = 0;
            menuParams.LastWave = 0;//Прерван
            menuParams.CompletedLevel = false; //Не завершен
            var exitParams = new GameplayExitParams(menuParams);
            _exitSceneRequest.OnNext(exitParams);
        }

        /**
         * Для тестирования
         */
        public void ExitSave()
        {
            var menuParams = new MainMenuEnterParams("Выход с сохранением");
            menuParams.GameSpeed = _gameplayState.PreviousGameSpeed;
            menuParams.SoftCurrency = _gameplayState.SoftCurrency.CurrentValue;
            var exitParams = new GameplayExitParams(menuParams);
            exitParams.SaveGameplay = true;
            _exitSceneRequest.OnNext(exitParams);
        }
    }
}