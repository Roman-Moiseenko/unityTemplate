using System;
using DI;
using Game.Common;
using Game.GamePlay.Classes;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.TowerStates;
using Game.GamePlay.Root;
using Game.GamePlay.Services;
using Game.State;
using Game.State.Maps.Rewards;
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

        public readonly ReactiveProperty<int> ProgressData = new();
        public readonly ReactiveProperty<int> ProgressLevel = new();
        public readonly ReactiveProperty<long> SoftCurrency = new();
        public readonly ReactiveProperty<long> HardCurrency;
        public readonly ReactiveProperty<string> WaveText = new();
        public ObservableList<DamageEntity> AllDamages;
        public ObservableList<RewardCurrencyEntity> AllRewards;
        public ObservableList<float> RepairBuffer;

        public ReactiveProperty<float> CastleHealth;
        public float CastleFullHealth;
        private readonly RewardProgressService _rewardService;

        public ReactiveProperty<bool> ShowStartWave = new(false);
        public ReactiveProperty<bool> ShowFinishWave = new(false);
        public ReactiveProperty<RewardEntity> RewardEntity;
        public override string Id => "ScreenGameplay";
        public override string Path => "Gameplay/ScreenGameplay/";
        private IDisposable _disposable;

        public ReactiveProperty<bool> ShowTopMenu = new(true);

        public ScreenGameplayViewModel(
            GameplayUIManager uiManager,
            Subject<GameplayExitParams> exitSceneRequest,
            DIContainer container
        ) : base(container)
        {
            var d = Disposable.CreateBuilder();
            _uiManager = uiManager;
            _exitSceneRequest = exitSceneRequest;
            _gameplayState = container.Resolve<IGameStateProvider>().GameplayState;
            _castleService = container.Resolve<CastleService>();
            _waveService = container.Resolve<WaveService>();
            _rewardService = container.Resolve<RewardProgressService>();
            var fsmTower = container.Resolve<FsmTower>();
            fsmTower.Fsm.StateCurrent.Subscribe(v =>
            {
                if (v.GetType() == typeof(FsmTowerDelete)) _uiManager.OpenPopupTowerDelete();
            }).AddTo(ref d);

            HardCurrency = container.Resolve<IGameStateProvider>().GameState.HardCurrency;
            AllRewards = _rewardService.RewardMaps;
            RewardEntity = _rewardService.RewardEntity;

            CastleHealth = _gameplayState.Castle.CurrenHealth;
            CastleFullHealth = _gameplayState.Castle.FullHealth;

            var damageService = container.Resolve<DamageService>();
            AllDamages = damageService.AllDamages;
            CameraService = container.Resolve<GameplayCamera>();
            RepairBuffer = _castleService.RepairBuffer;
            PositionCamera = container.Resolve<Subject<Unit>>(AppConstants.CAMERA_MOVING);
            WaveText.Value = 1 + "/" + _gameplayState.CountWaves;

            _waveService.FinishWave.Where(v => v).Subscribe(v =>
            {
                ShowFinishWave.Value = true;
                _waveService.FinishWave.Value = false; //Сбрасываем флаг окончания волны
            }).AddTo(ref d);
            _waveService.StartWave.Where(v => v).Subscribe(v =>
            {
                ShowStartWave.Value = true;
                _waveService.StartWave.Value = false; //Сбрасываем флаг окончания волны
                //Меняем номер волны
                WaveText.Value = _gameplayState.CurrentWave.CurrentValue + "/" + _gameplayState.CountWaves;
            }).AddTo(ref d);
            
            _gameplayState.Progress.Subscribe(newValue => ProgressData.Value = newValue).AddTo(ref d);
            _gameplayState.ProgressLevel.Subscribe(newValue => ProgressLevel.Value = newValue).AddTo(ref d);
            _gameplayState.SoftCurrency.Subscribe(newValue => SoftCurrency.Value = newValue).AddTo(ref d);
            _gameplayState.Castle.IsDead.Where(e => e)
                .Subscribe(newValue =>
                {
                    if (_gameplayState.Castle.CountResurrection.CurrentValue < 2)
                    {
                        _uiManager.OpenPopupLose();                        
                    }
                    
                })
                .AddTo(ref d);
            _disposable = d.Build();
        }

        public void RequestOpenPopupPause()
        {
            _uiManager.OpenPopupPause();
        }

        public override void Dispose()
        {
            _disposable.Dispose();
        }
    }
}