using System;
using System.Collections;
using System.Collections.Generic;
using DI;
using Game.Common;
using Game.GamePlay.Classes;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.WaveStates;
using Game.GamePlay.Services;
using Game.GamePlay.View.Towers;
using Game.Settings;
using Game.Settings.Gameplay.Enemies;
using Game.State;
using Game.State.Maps.Mobs;
using Game.State.Maps.Towers;
using MVVM.UI;
using ObservableCollections;
using R3;
using Scripts.Utils;
using UnityEngine;

namespace Game.GamePlay.View.UI.PanelGateWave
{
    public class PanelGateWaveViewModel : WindowViewModel
    {
        private readonly GameplayUIManager _uiManager;
        private readonly WaveService _waveService;
        private readonly Coroutines _coroutines;
        public override string Id => "PanelGateWave";
        public override string Path => "Gameplay/Panels/GateWaveInfo/";
        public readonly int CurrentSpeed;
        
        //Состояния отображения окон
        public ReactiveProperty<bool> ShowButtonWave = new(true);
        public ReactiveProperty<bool> ShowInfoWave = new(false);
        public ReactiveProperty<bool> ShowInfoTower = new(false);
        
        public ReactiveProperty<Vector3> PositionInfoBtn = new(Vector3.zero);
        public ReactiveProperty<Vector3> PositionInfoTower = new(Vector3.zero);
        public ReactiveProperty<float> FillAmountBtn = new(1f);
        private readonly GameplayCamera _cameraService;
        
        //Словари префабов для отображения в Binder
        public ObservableDictionary<TowerParameterType, float> BaseParameters = new(); 
        public ObservableDictionary<TowerParameterType, float> UpgradeParameters = new();
            
        private Vector2Int _towerPrevious = Vector2Int.zero;
        public TowerViewModel TowerViewModel;
        private readonly IDisposable _disposable;
        
        public ObservableDictionary<string, int> InfoWaveMobs = new(); //Информация о мобе в волн
        public MobsSettings MobsSettings { get; private set; }
        
        public PanelGateWaveViewModel(
            GameplayUIManager uiManager, 
            DIContainer container
            )
        {
            var d = Disposable.CreateBuilder();
            
            _uiManager = uiManager;
            _waveService = container.Resolve<WaveService>();
            _coroutines = GameObject.Find("[COROUTINES]").GetComponent<Coroutines>();
            
            var entityClick = container.Resolve<Subject<Unit>>(AppConstants.CLICK_WORLD_ENTITY);
            var towerClick = container.Resolve<Subject<TowerViewModel>>();
            var fsmWave = container.Resolve<FsmWave>();
            var gameplayStateProxy = container.Resolve<IGameStateProvider>().GameplayState;
            var positionCamera = container.Resolve<Subject<Unit>>(AppConstants.CAMERA_MOVING);

            CurrentSpeed = gameplayStateProxy.GetCurrentSpeed();
            MobsSettings = container.Resolve<ISettingsProvider>().GameSettings.MobsSettings;            
            
            entityClick.Subscribe(_ =>
            {
                ShowInfoWave.OnNext(false);
                ShowInfoTower.OnNext(false);
            }).AddTo(ref d);
            towerClick.Subscribe(towerViewModel =>
            {
                if (_towerPrevious == towerViewModel.Position.CurrentValue)
                {
                    ShowInfoTower.OnNext(false);
                    _towerPrevious = Vector2Int.zero;
                }
                else
                {
                    TowerViewModel = towerViewModel;
                    _towerPrevious = towerViewModel.GetPosition();
                    BaseParameters.Clear();
                    UpgradeParameters.Clear();
                    
                    foreach (var parameterData in towerViewModel.Parameters)
                    {
                        BaseParameters.Add(parameterData.Key, parameterData.Value.Value);
                    }
                    
                    NewPositionTowerInfo();
                    ShowInfoTower.OnNext(true);
                }
            }).AddTo(ref d);
            fsmWave.Fsm.StateCurrent.Subscribe(state =>
            {
                if (state.GetType() == typeof(FsmStateWaveBegin))
                {
                    ShowButtonWave.OnNext(false);
                }

                if (state.GetType() == typeof(FsmStateWaveWait))
                {
                    ShowButtonWave.OnNext(true);
                }
            }).AddTo(ref d);
            gameplayStateProxy.CurrentWave.Subscribe(number =>
            {
                InfoWaveMobs.Clear();
                foreach (var keyPair in gameplayStateProxy.Waves[number].GetInfoMobsFromWave())
                {
                    InfoWaveMobs.Add(keyPair.Key, keyPair.Value);
                }
            }).AddTo(ref d);
            //Изменилась позиция камеры
            positionCamera.Subscribe(n =>
            {
                NewPositionButtonInfo();
                if (ShowInfoTower.CurrentValue) NewPositionTowerInfo();
            }).AddTo(ref d);
            
            _waveService.TimeOutNewWaveValue.
                Subscribe(n => FillAmountBtn.Value = 1 - n)
                .AddTo(ref d);
            _cameraService = container.Resolve<GameplayCamera>();
            //Изменилась позиция ворот
            _waveService.GateWaveViewModel.Position.Subscribe(_ => NewPositionButtonInfo()).AddTo(ref d);
            
            _disposable = d.Build();
        }

        private void NewPositionButtonInfo()
        {
            var position = _waveService.GateWaveViewModel.Position.CurrentValue;
            var p = new Vector3(position.x, 0, position.y);
            var v= _cameraService.Camera.WorldToScreenPoint(p);
            v.z = 0;
            PositionInfoBtn.Value = v;
        }

        private void NewPositionTowerInfo()
        {
            var p = new Vector3(_towerPrevious.x, 0, _towerPrevious.y);
            var v= _cameraService.Camera.WorldToScreenPoint(p);
            v.z = 0;
            v.y += 100;
            PositionInfoTower.Value = v;
        }
        
        public void StartForcedWave()
        {
            _waveService.StartForcedNewWave();
        }
        
        public override void Dispose()
        {
            _disposable.Dispose();
        }
    }
}