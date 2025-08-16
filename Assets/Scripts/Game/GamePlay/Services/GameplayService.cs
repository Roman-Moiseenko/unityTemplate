using System;
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
    public class GameplayService : IDisposable
    {
        private readonly Subject<GameplayExitParams> _exitSceneRequest;
        private readonly DIContainer _container;
        private readonly GameplayStateProxy _gameplayState;

        private IDisposable _disposable;
        // public ReactiveProperty<float> CastleHealth;

        public GameplayService(
            Subject<GameplayExitParams> exitSceneRequest,
            WaveService waveService,
            GameplayStateProxy gameplayState)
        {
            var d = Disposable.CreateBuilder();
            _exitSceneRequest = exitSceneRequest;
            _gameplayState = gameplayState;

            waveService.IsMobsOnWay.Where(flag => flag == false).Subscribe(_ =>
                {
                    //Мобы на дороге закончились, проверяем закончились ли волны. 
                    if (_gameplayState.IsFinishWaves()) Win();
                }
            ).AddTo(ref d);

            //Сработала следующая волна, после максимальной => Победа

            
            //Здоровье крепости меньше 0 => Проигрыш
            //gameplayState.Castle.IsDead.Subscribe(h => Lose()).AddTo(ref d);
            _disposable = d.Build();
        }


        public void Win() //private
        {
            var menuParams = new MainMenuEnterParams("Выход");
            //Передаем награду и некоторые настройки, для загрузки в меню
            menuParams.GameSpeed = _gameplayState.PreviousGameSpeed;
            menuParams.SoftCurrency = _gameplayState.SoftCurrency.CurrentValue;
            //menuParams.RewardCards = _gameplayState.RewardCards;
            menuParams.CompletedLevel = true;
            var exitParams = new GameplayExitParams(menuParams);
            _exitSceneRequest.OnNext(exitParams);
        }

        public void Lose() //private
        {
            var menuParams = new MainMenuEnterParams("Выход");
            //Передаем награду и некоторые настройки, для загрузки в меню
            menuParams.GameSpeed = _gameplayState.PreviousGameSpeed;
            menuParams.SoftCurrency = _gameplayState.SoftCurrency.CurrentValue;
            //menuParams.RewardCards = _gameplayState.RewardCards;
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
            menuParams.RewardCards = null;
            menuParams.LastWave = 0; //Прерван
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
            //menuParams.RewardCards = _gameplayState.RewardCards;
            var exitParams = new GameplayExitParams(menuParams);
            exitParams.SaveGameplay = true;
            _exitSceneRequest.OnNext(exitParams);
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}