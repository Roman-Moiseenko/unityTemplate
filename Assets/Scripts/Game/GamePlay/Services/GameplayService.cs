using System;
using DI;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.States;
using Game.GamePlay.Root;
using Game.MainMenu.Root;
using Game.State;
using Game.State.Root;
using R3;
using Scripts.Game.GameRoot.Entity;
using Scripts.Game.GameRoot.Services;
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
        private readonly AdService _adService;
        private WaveService _waveService;
        private FsmGameplay _fsmGameplay;
        
        
        private IDisposable _disposable;
        // public ReactiveProperty<float> CastleHealth;

        public GameplayService(
            Subject<GameplayExitParams> exitSceneRequest,
            WaveService waveService,
            GameplayStateProxy gameplayState,
            AdService adService,
            FsmGameplay fsmGameplay)
        {
            var d = Disposable.CreateBuilder();
            _exitSceneRequest = exitSceneRequest;
            _gameplayState = gameplayState;
            _adService = adService;
            _fsmGameplay = fsmGameplay;
            _waveService = waveService;
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

        public void RepairCristal()
        {
            //TODO Тратим кристалы, если нет PopupError
            Repair();
            
        }

        public void RepairAd()
        {
            //TODO 

            var ad = _adService.ShowAdGoogle();
            _fsmGameplay.Fsm.SetState<FsmStateGamePause>();

            ad.CloseShow.Subscribe(v =>
            {
                if (v.Success.CurrentValue)
                {
                    Repair();
                    _fsmGameplay.Fsm.SetState<FsmStateGamePlay>();
                }
                else
                {
                    Lose();
                }
            });
        }

        private void Repair()
        {
            _gameplayState.Castle.Resurrection();
            foreach (var entity  in _waveService.AllMobsMap)
            {
                entity.Value.SetDamage(_gameplayState.Castle.FullHealth);
            }
        }
    }
}