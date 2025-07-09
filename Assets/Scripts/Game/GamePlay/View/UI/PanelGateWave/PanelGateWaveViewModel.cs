using System.Collections;
using DI;
using Game.Common;
using Game.GamePlay.Classes;
using Game.GamePlay.Services;
using Game.State;
using Game.State.Root;
using MVVM.UI;
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
        public override string Path => "Gameplay/";
        public readonly int CurrentSpeed;
        private readonly GameplayStateProxy _gameplayStateProxy;
        public ReactiveProperty<bool> StartForced;
        public ReactiveProperty<bool> ShowGate;
        public ReactiveProperty<Vector3> PositionInfoBtn = new(Vector3.zero);
        public ReactiveProperty<float> FillAmountBtn = new(1f);
        
        public PanelGateWaveViewModel(
            GameplayUIManager uiManager, 
            DIContainer container
            )
        {
            _uiManager = uiManager;
            _container = container;
            _waveService = container.Resolve<WaveService>();
            _coroutines = GameObject.Find("[COROUTINES]").GetComponent<Coroutines>();
            _gameplayStateProxy = container.Resolve<IGameStateProvider>().GameplayState;
            
            StartForced = _waveService.StartForced;
            CurrentSpeed = _gameplayStateProxy.GetCurrentSpeed();
            ShowGate = _waveService.ShowGate;
            _waveService.TimeOutNewWaveValue.Subscribe(n => FillAmountBtn.Value = 1 - n);
            var cameraService = container.Resolve<GameplayCamera>();
            
            var positionCamera = container.Resolve<Subject<Unit>>(AppConstants.CAMERA_MOVING);
            positionCamera.Subscribe(n =>
            {
                //Изменилась позиция, вычисляем координаты
                var position = _waveService.GateWaveViewModel.Position.CurrentValue;
                var p = new Vector3(position.x, 0, position.y);
                var v= cameraService.Camera.WorldToScreenPoint(p);
                v.z = 0;
                PositionInfoBtn.Value = v;
            });
        }



        public void StartForcedWave()
        {
            _waveService.StartForcedNewWave();
            
            //TODO Ускоряем таймер Сделать
        }

        public void ShowPopupInfo()
        {
            //TODO Открываем popup окно с информацией о волне, передать данные из WaveService
        }
    }
}