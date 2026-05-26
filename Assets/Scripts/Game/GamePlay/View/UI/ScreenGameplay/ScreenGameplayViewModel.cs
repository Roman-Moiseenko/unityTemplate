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
        
        
        public readonly GameplayCamera CameraService;
        public readonly Subject<Unit> PositionCamera;

        public readonly ReactiveProperty<int> ProgressData = new();
        public readonly ReactiveProperty<int> ProgressLevel = new();
        public readonly ReactiveProperty<long> SoftCurrency = new();
        public readonly ReactiveProperty<long> HardCurrency;
        public readonly ReactiveProperty<string> WaveText = new();
        public readonly ObservableList<DamageEntity> AllDamages;
        public readonly ObservableList<RewardCurrencyEntity> AllRewards;
        public readonly ObservableList<float> RepairBuffer;

        public readonly ReactiveProperty<float> CastleHealth;
        public readonly float CastleFullHealth;

        public readonly ReactiveProperty<bool> ShowStartWave = new(false);
        public readonly ReactiveProperty<bool> ShowFinishWave = new(false);
        public readonly ReactiveProperty<RewardEntity> RewardEntity;
        public override string Id => "ScreenGameplay";
        public override string Path => "Gameplay/ScreenGameplay/";

        public readonly ReactiveProperty<bool> ShowTopMenu = new(true);
        
        private readonly GameplayUIManager _uiManager;
        //TODO Возможно удалить
        private readonly Subject<GameplayExitParams> _exitSceneRequest;

        public ScreenGameplayViewModel(
            GameplayUIManager uiManager,
            Subject<GameplayExitParams> exitSceneRequest,
            DIContainer container
        ) : base(container)
        {
            var gameplayState = container.Resolve<IGameStateProvider>().GameplayState;
            var castleService = container.Resolve<CastleService>();
            var waveService = container.Resolve<WaveService>();
            var rewardService = container.Resolve<RewardProgressService>();
            var fsmTower = container.Resolve<FsmTower>();
            var damageService = container.Resolve<DamageService>();
            
            _uiManager = uiManager;
            _exitSceneRequest = exitSceneRequest;            
            
            HardCurrency = container.Resolve<IGameStateProvider>().GameState.HardCurrency;
            AllRewards = rewardService.RewardMaps;
            RewardEntity = rewardService.RewardEntity;
            CastleHealth = gameplayState.Castle.CurrenHealth;
            CastleFullHealth = gameplayState.Castle.FullHealth;
            AllDamages = damageService.AllDamages;
            CameraService = container.Resolve<GameplayCamera>();
            RepairBuffer = castleService.RepairBuffer;
            PositionCamera = container.Resolve<Subject<Unit>>(AppConstants.CAMERA_MOVING);
            WaveText.Value = 1 + "/" + gameplayState.CountWaves;
            
            fsmTower.Fsm.StateCurrent.Subscribe(v =>
            {
                if (v.GetType() == typeof(FsmTowerDelete)) _uiManager.OpenPopupTowerDelete();
            }).AddTo(ref _disposables);
            
            waveService.FinishWave.Where(v => v).Subscribe(v =>
            {
                ShowFinishWave.Value = true;
                waveService.FinishWave.Value = false; //Сбрасываем флаг окончания волны
            }).AddTo(ref _disposables);
            waveService.StartWave.Where(v => v).Subscribe(v =>
            {
                ShowStartWave.Value = true;
                waveService.StartWave.Value = false; //Сбрасываем флаг окончания волны
                //Меняем номер волны
                WaveText.Value = gameplayState.CurrentWave.CurrentValue + "/" + gameplayState.CountWaves;
            }).AddTo(ref _disposables);
            
            gameplayState.Progress
                .Subscribe(newValue => ProgressData.Value = newValue)
                .AddTo(ref _disposables);
            gameplayState.ProgressLevel
                .Subscribe(newValue => ProgressLevel.Value = newValue)
                .AddTo(ref _disposables);
            gameplayState
                .SoftCurrency
                .Subscribe(newValue => SoftCurrency.Value = newValue).AddTo(ref _disposables);
            gameplayState.Castle.IsDead
                .Where(e => e)
                .Subscribe(newValue =>
                {
                    if (gameplayState.Castle.CountResurrection.CurrentValue < 2)
                        _uiManager.OpenPopupLose();                        
                })
                .AddTo(ref _disposables);
        }

        public void RequestOpenPopupPause()
        {
            _uiManager.OpenPopupPause();
        }

        public override void Dispose()
        {
            ShowStartWave?.Dispose();
            ShowFinishWave?.Dispose();
            ShowTopMenu?.Dispose();
            base.Dispose();
        }
    }
}