using System;
using System.Collections;
using System.Collections.Generic;
using DI;
using Game.Common;
using Game.GamePlay.Classes;
using Game.GamePlay.Services;
using Game.GamePlay.View.Towers;
using Game.Settings;
using Game.Settings.Gameplay.Enemies;
using Game.State;
using Game.State.Maps.Mobs;
using Game.State.Maps.Towers;
using Game.State.Root;
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
        private readonly DIContainer _container;
        private WaveService _waveService;
        private readonly Coroutines _coroutines;
        public override string Id => "PanelGateWave";
        public override string Path => "Gameplay/Panels/GateWaveInfo/";
        public readonly int CurrentSpeed;
        private readonly GameplayStateProxy _gameplayStateProxy;
        public ReactiveProperty<bool> StartForced;
        
        //Состояния отображения окон
        public ReactiveProperty<bool> ShowButtonWave;
        public ReactiveProperty<bool> ShowInfoWave = new(false);
        public ReactiveProperty<bool> ShowInfoTower = new(false);
        
        public ReactiveProperty<Vector3> PositionInfoBtn = new(Vector3.zero);
        public ReactiveProperty<Vector3> PositionInfoTower = new(Vector3.zero);
        public ReactiveProperty<float> FillAmountBtn = new(1f);
        private GameplayCamera _cameraService;
        
        //Словари префабов для отображения в Binder
        public ObservableDictionary<TowerParameterType, float> BaseParameters = new(); 
        public ObservableDictionary<TowerParameterType, float> UpgradeParameters = new();
            
        private Vector2Int _towerPrevious = Vector2Int.zero;
        public TowerViewModel TowerViewModel;
        private IDisposable _disposable;
        
        public ObservableDictionary<string, int> InfoWaveMobs = new(); //Информация о мобе в волн
        public MobsSettings MobsSettings { get; private set; }
        
        public PanelGateWaveViewModel(
            GameplayUIManager uiManager, 
            DIContainer container
            )
        {
            var d = Disposable.CreateBuilder();
            _uiManager = uiManager;
            _container = container;
            _waveService = container.Resolve<WaveService>();
            _coroutines = GameObject.Find("[COROUTINES]").GetComponent<Coroutines>();
            _gameplayStateProxy = container.Resolve<IGameStateProvider>().GameplayState;
            var entityClick = container.Resolve<Subject<Unit>>(AppConstants.CLICK_WORLD_ENTITY);
            var towerClick = container.Resolve<Subject<TowerViewModel>>();
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
                    //TODO Наполнение UpgradeParameters - ??? по Level и настройкам вычисляем %% Upgrade
                    NewPositionTowerInfo();
                    ShowInfoTower.OnNext(true);
                }
            }).AddTo(ref d);

            _gameplayStateProxy.CurrentWave.Subscribe(number =>
            {
                InfoWaveMobs.Clear();
                foreach (var keyPair in _gameplayStateProxy.Waves[number].GetInfoMobsFromWave())
                {
                    InfoWaveMobs.Add(keyPair.Key, keyPair.Value);
                }
            }).AddTo(ref d);
            
            //TODO Переделать в машину состояния
            StartForced = _waveService.StartForced;
            ShowButtonWave = _waveService.ShowInfoWave;
            
            CurrentSpeed = _gameplayStateProxy.GetCurrentSpeed();
            _waveService.TimeOutNewWaveValue.Subscribe(n => FillAmountBtn.Value = 1 - n).AddTo(ref d);
            _cameraService = container.Resolve<GameplayCamera>();
            
            
            var positionCamera = container.Resolve<Subject<Unit>>(AppConstants.CAMERA_MOVING);
            //Изменилась позиция ворот
            _waveService.GateWaveViewModel.Position.Subscribe(_ => NewPositionButtonInfo()).AddTo(ref d);
            //Изменилась позиция камеры
            positionCamera.Subscribe(n =>
            {
                NewPositionButtonInfo();
                if (ShowInfoTower.CurrentValue) NewPositionTowerInfo();
            }).AddTo(ref d);
            
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

        public void ShowPopupInfo()
        {
            //TODO Открываем popup окно с информацией о волне, передать данные из WaveService
        }
        
        public override void Dispose()
        {
            _disposable.Dispose();
        }
    }
}