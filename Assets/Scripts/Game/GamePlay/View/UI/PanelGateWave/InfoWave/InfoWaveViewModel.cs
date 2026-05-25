using System;
using System.Collections.Generic;
using DI;
using Game.Common;
using Game.GamePlay.Classes;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.WaveStates;
using Game.GamePlay.Queries.Classes;
using Game.GamePlay.Queries.WaveQueries;
using Game.GamePlay.Services;
using Game.State;
using MVVM.CMD;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.UI.PanelGateWave.InfoWave
{
    public class InfoWaveViewModel : IDisposable
    {
        //Состояния отображения окон
        public readonly ReactiveProperty<bool> ShowButtonWave = new(true);
        public readonly ReactiveProperty<bool> ShowInfoWave = new(false);
        public readonly ObservableList<EnemyDataInfo> AllEnemyDataInfo = new(); //Информация о мобе в волн
        public readonly ReactiveProperty<float> FillAmountBtn = new(1f);
        public readonly ReactiveProperty<Vector3> PositionInfoBtn = new(Vector3.zero);
        private readonly WaveService _waveService;
        private readonly GameplayCamera _cameraService;
        private DisposableBag _disposables;
        
        public InfoWaveViewModel(DIContainer container, bool isWay = true)
        {
            _waveService = container.Resolve<WaveService>();
            _cameraService = container.Resolve<GameplayCamera>();
            
            var qrc = container.Resolve<IQueryProcessor>();
            var fsmWave = container.Resolve<FsmWave>();
            var entityClick = container.Resolve<Subject<Unit>>(AppConstants.CLICK_WORLD_ENTITY);
            var gameplayState = container.Resolve<IGameStateProvider>().GameplayState;
            var positionCamera = container.Resolve<Subject<Unit>>(AppConstants.CAMERA_MOVING);

            //Закрываем окно, если нажатие за пределами UI
            entityClick.Subscribe(_ => ShowInfoWave.Value = false).AddTo(ref _disposables);

            //Показываем/скрываем кнопку Волны в зависимости от состояния волны
            fsmWave.Fsm.StateCurrent.Subscribe(state =>
            {
                if (state.GetType() == typeof(FsmStateWaveBegin)) ShowButtonWave.OnNext(false);
                if (state.GetType() == typeof(FsmStateWaveWait)) ShowButtonWave.OnNext(true);
            }).AddTo(ref _disposables);
            //При изменении счетчика текущей волны, получаем информацию о след.волне
            // Skip(1) - пропускаем начальное значение CurrentWave = 0 (волна еще не началась),
            // иначе NumberWave - 1 = -1 вызовет ArgumentOutOfRangeException в QueryInfoWaveHandler
            gameplayState.CurrentWave.Skip(1).Subscribe(number =>
            {
                AllEnemyDataInfo.Clear();
                //Получить данные О Новой Волне
                var query = new QueryInfoWave { NumberWave = number, IsWay = isWay};
                var list = (List<EnemyDataInfo>)qrc.Request(query);
                foreach (var dataInfo in list)
                {
                    AllEnemyDataInfo.Add(dataInfo);
                }
            }).AddTo(ref _disposables);
            //Изменилась позиция камеры
            positionCamera.Subscribe(n => NewPositionButtonInfo(isWay)).AddTo(ref _disposables);
            _waveService.TimeOutNewWaveValue.
                Subscribe(n => FillAmountBtn.Value = 1 - n)
                .AddTo(ref _disposables);
            
            //Изменилась позиция ворот
            _waveService.GateWaveViewModel.Position
                .Subscribe(_ => NewPositionButtonInfo(isWay))
                .AddTo(ref _disposables);
        }
        
        private void NewPositionButtonInfo(bool isWay)
        {
            var position = isWay 
                ? _waveService.GateWaveViewModel.Position.CurrentValue 
                : _waveService.GateWaveSecondViewModel.Position.CurrentValue;
            var p = new Vector3(position.x, 0, position.y);
            var v= _cameraService.Camera.WorldToScreenPoint(p);
            v.z = 0;
            PositionInfoBtn.Value = v;
        }
        
        public void StartForcedWave()
        {
            _waveService.StartForcedNewWave();
        }

        public void Dispose()
        {
            ShowButtonWave?.Dispose();
            ShowInfoWave?.Dispose();
            FillAmountBtn?.Dispose();
            PositionInfoBtn?.Dispose();
            _disposables.Dispose();
        }
    }
}