﻿using DI;
using Game.Common;
using Game.GamePlay.Classes;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.States;
using Game.GamePlay.Root;
using Game.GamePlay.Services;
using Game.MainMenu.Services;
using Game.Settings;
using Game.State;
using Game.State.GameResources;
using Game.State.Maps.Shots;
using Game.State.Root;
using MVVM.UI;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.UI.ScreenGameplay
{
    public class ScreenGameplayViewModel : WindowViewModel
    {
        public readonly GameplayUIManager _uiManager;

        public GameplayCamera CameraService;

        public Subject<Unit> PositionCamera;
        //TODO Возможно удалить
        private readonly Subject<GameplayExitParams> _exitSceneRequest;
        private readonly GameplayStateProxy _gameplayState;
        private readonly CastleService _castleService;
        private WaveService _waveService;
        
        //TODO Данные для Binder, возможно заменить в дальнейшем прогрузкой анимации и др.
        public readonly ReactiveProperty<int> ProgressData = new();
        public readonly ReactiveProperty<int> SoftCurrency = new();
        public readonly ReactiveProperty<int> HardCurrency = new();
        public readonly ReactiveProperty<string> WaveText = new();
        public ObservableList<DamageEntity> AllDamages = new();
        public ObservableList<float> RepairBuffer;

        public ReactiveProperty<float> CastleHealth;
        public float CastleFullHealth;
   
        public override string Id => "ScreenGameplay";
        public override string Path => "Gameplay/ScreenGameplay/";
     //   public readonly int CurrentSpeed;
        public ScreenGameplayViewModel(
            GameplayUIManager uiManager, 
            Subject<GameplayExitParams> exitSceneRequest,
            DIContainer container
            )
        {
            _uiManager = uiManager;
            _exitSceneRequest = exitSceneRequest;
            _gameplayState = container.Resolve<IGameStateProvider>().GameplayState;
            _castleService = container.Resolve<CastleService>();
            _waveService = container.Resolve<WaveService>();

            CastleHealth = _gameplayState.Castle.CurrenHealth;
            CastleFullHealth = _gameplayState.Castle.FullHealth;
            
            
            var damageService = container.Resolve<DamageService>();
            AllDamages = damageService.AllDamages;             
            CameraService = container.Resolve<GameplayCamera>();
            RepairBuffer = _castleService.RepairBuffer;
            PositionCamera = container.Resolve<Subject<Unit>>(AppConstants.CAMERA_MOVING);
            
            _gameplayState.CurrentWave.Subscribe(n =>
            {
                WaveText.Value = n + "/" + _gameplayState.Waves.Count;
            });

            //_waveService.
            //_gameplayState = container.Resolve<IGameStateProvider>().GameState;
            _gameplayState.Progress.Subscribe(newValue => ProgressData.Value = newValue);
            _gameplayState.SoftCurrency.Subscribe(newValue => SoftCurrency.Value = newValue);
            
        }
        
        public void RequestOpenPopupPause()
        {
            _uiManager.OpenPopupPause();
        }
        
        public void RequestOpenPopupB()
        {
            _uiManager.OpenPopupB();
        }
        

    }
}