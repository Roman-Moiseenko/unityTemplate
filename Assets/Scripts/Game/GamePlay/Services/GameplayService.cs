using System;
using System.Linq;
using DI;
using Game.Common;
using Game.GamePlay.Classes;
using Game.GamePlay.Commands.WaveCommands;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.States;
using Game.GamePlay.Root;
using Game.GameRoot.Services;
using Game.MainMenu.Root;
using Game.State;
using Game.State.Maps.Mobs;
using Game.State.Root;
using MVVM.CMD;
using R3;
using Scripts.Game.GameRoot.Entity;
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
        private readonly ResourceService _resourceService;
        private readonly ICommandProcessor _cmd;


        private IDisposable _disposable;
        // public ReactiveProperty<float> CastleHealth;

        public GameplayService(
            Subject<GameplayExitParams> exitSceneRequest,
            WaveService waveService,
            GameplayStateProxy gameplayState,
            AdService adService,
            FsmGameplay fsmGameplay,
            ResourceService resourceService,
            ICommandProcessor cmd
        )
        {
            var d = Disposable.CreateBuilder();
            _exitSceneRequest = exitSceneRequest;
            _gameplayState = gameplayState;
            _adService = adService;
            _fsmGameplay = fsmGameplay;
            _waveService = waveService;
            _resourceService = resourceService;
            _cmd = cmd;

            waveService.IsMobsOnWay.Where(flag => flag == false).Subscribe(_ =>
                {
                    //Мобы на дороге закончились, проверяем закончились ли волны. 
                    if (_gameplayState.IsFinishWaves()) Win();
                }
            ).AddTo(ref d);

            //Для бесконечной игры добавляем автоувеличение уровня
            if (_gameplayState.IsInfinity())
            {
                _gameplayState.CurrentWave.Skip(1).Where(v => v == _gameplayState.Waves.Count).Subscribe(v =>
                {
                    var command = new CommandWaveGenerate(v + 1);
                    cmd.Process(command);
                }).AddTo(ref d);
            }
            

            //Сработала следующая волна, после максимальной => Победа
            
            //Здоровье крепости меньше 0 => Проигрыш
            //gameplayState.Castle.IsDead.Subscribe(h => Lose()).AddTo(ref d);
            _disposable = d.Build();
        }


        public void Win() //private
        {
            var menuParams = new MainMenuEnterParams("Выход");
            //Передаем награду и некоторые настройки, для загрузки в меню
            menuParams.GameSpeed = _gameplayState.GetLastSpeedGame();
            menuParams.SoftCurrency = _gameplayState.SoftCurrency.CurrentValue;
            menuParams.MapId = _gameplayState.MapId.CurrentValue;
            //menuParams.RewardCards = _gameplayState.RewardCards;
            menuParams.CompletedLevel = true;
            var exitParams = new GameplayExitParams(menuParams);
            _exitSceneRequest.OnNext(exitParams);
        }

        public void Lose() //private
        {
            var menuParams = new MainMenuEnterParams("Выход");
            //Передаем награду и некоторые настройки, для загрузки в меню
            menuParams.GameSpeed = _gameplayState.GetLastSpeedGame();
            menuParams.SoftCurrency = _gameplayState.SoftCurrency.CurrentValue;
            menuParams.MapId = _gameplayState.MapId.CurrentValue;
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
            menuParams.GameSpeed = _gameplayState.GetLastSpeedGame();
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
            menuParams.GameSpeed = _gameplayState.GetLastSpeedGame();
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
            if (_resourceService.SpendHardCurrency(AppConstants.COST_REPAIR_CASTLE)) Repair();
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
            foreach (var entity in _waveService.AllMobsMap.ToArray())
            {
                entity.Value.SetDamage(_gameplayState.Castle.FullHealth);
            }
        }
    }
}