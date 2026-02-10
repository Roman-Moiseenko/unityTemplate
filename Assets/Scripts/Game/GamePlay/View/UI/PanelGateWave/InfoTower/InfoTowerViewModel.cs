using System;
using System.Collections.Generic;
using DI;
using Game.Common;
using Game.GamePlay.Classes;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.TowerStates;
using Game.GamePlay.Root;
using Game.GamePlay.Services;
using Game.GamePlay.View.Towers;
using Game.Settings;
using Game.Settings.Gameplay.Entities.Tower;
using Game.State.Inventory;
using Game.State.Maps.Mobs;
using Game.State.Maps.Towers;
using Game.State.Research;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.UI.PanelGateWave.InfoTower
{
    public class InfoTowerViewModel
    {
        public ReactiveProperty<bool> ShowInfoTower = new(false);
        public ReactiveProperty<bool> UpdateInfoBackgroundTower = new(false);
        public ReactiveProperty<Vector3> PositionInfoTower = new(Vector3.zero);
        private readonly GameplayCamera _cameraService;
        
        public Dictionary<TowerParameterType, float> BaseParameters = new(); 
        public Dictionary<TowerParameterType, float> BoosterParameters = new();
        
        private Vector2Int _towerPrevious = Vector2Int.zero;
        public TowerViewModel TowerViewModel;

        public string NameTower;
        public TypeEpicCard EpicLevel;
        public int Level;
        public MobDefence Defence;
        
        private readonly GameSettings _gameSettings;
        private readonly List<TowerSettings> _settingsTowers;
        private readonly GameplayBoosters _gameplayBooster;
        private readonly Dictionary<string,Dictionary<TowerParameterType,float>> _towerBoosters;

        public InfoTowerViewModel(DIContainer container)
        {
            var positionCamera = container.Resolve<Subject<Unit>>(AppConstants.CAMERA_MOVING);
            var fsmTower = container.Resolve<FsmTower>();
            var gameSettings = container.Resolve<ISettingsProvider>().GameSettings;
            _settingsTowers = gameSettings.TowersSettings.AllTowers;
            
            _gameplayBooster = container.Resolve<GameplayEnterParams>().GameplayBoosters;
            _towerBoosters = container.Resolve<TowersService>().TowerBoosters;
                
            fsmTower.Fsm.StateCurrent.Subscribe(state =>
            {
                if (state.GetType() == typeof(FsmTowerNone) || 
                    state.GetType() == typeof(FsmTowerDelete) ||
                    state.GetType() == typeof(FsmTowerPlacement))
                {
                    ShowInfoTower.Value = false;
                    _towerPrevious = Vector2Int.zero;
                }
                if (state.GetType() == typeof(FsmTowerSelected))
                {
                    PrepareAndShowInfo(fsmTower.GetTowerViewModel());
                }
                //TODO Сделать для других состояний
            });
            
            _cameraService = container.Resolve<GameplayCamera>();
 
            //Изменилась позиция камеры
            positionCamera.Subscribe(n =>
            {
                if (ShowInfoTower.CurrentValue) NewPositionTowerInfo();
            });
        }
        
        private void NewPositionTowerInfo()
        {
            var p = new Vector3(_towerPrevious.x - 0.5f, 1f, _towerPrevious.y - 0.5f);
            var v= _cameraService.Camera.WorldToScreenPoint(p);
            //v.z = 0;
            //v.y += 100;
            PositionInfoTower.Value = v;
        }

        private void PrepareAndShowInfo(TowerViewModel towerViewModel)
        {
            if (_towerPrevious == towerViewModel.Position.CurrentValue)
            {
                if (ShowInfoTower.CurrentValue) ShowInfoTower.OnNext(false);
                _towerPrevious = Vector2Int.zero;
            }
            else
            {
                TowerViewModel = towerViewModel;

                var config = _settingsTowers.Find(t => t.ConfigId == towerViewModel.ConfigId);
                NameTower = config.TitleLid;
                Defence = config.Defence;
                EpicLevel = towerViewModel.EpicLevel;
                Level = towerViewModel.Level.CurrentValue;
                
                
                _towerPrevious = towerViewModel.GetPosition();
                BaseParameters.Clear();
                BoosterParameters.Clear();
//                Debug.Log(" **** " + _towerBoosters[towerViewModel.ConfigId].Count);
                foreach (var (parameterType, value) in _towerBoosters[towerViewModel.ConfigId])
                {
                    //TODO Для всех Damage сделать проверка
                    if (parameterType == TowerParameterType.Damage)
                    {
                        if (towerViewModel.Parameters.TryGetValue(TowerParameterType.Damage, out _))
                            BoosterParameters.Add(TowerParameterType.Damage, value);
                        if (towerViewModel.Parameters.TryGetValue(TowerParameterType.DamageArea, out _))
                            BoosterParameters.Add(TowerParameterType.DamageArea, value);
                    }
                    else
                    {
                        BoosterParameters.Add(parameterType, value);    
                    }
                    
                }
                
                
                //Урон, все 3
                if (towerViewModel.Parameters.TryGetValue(TowerParameterType.Damage, out var damage))
                    BaseParameters.Add(TowerParameterType.Damage, damage.Value);
                if (towerViewModel.Parameters.TryGetValue(TowerParameterType.DamageArea, out var damageArea))
                    BaseParameters.Add(TowerParameterType.DamageArea, damageArea.Value);
                //TODO Добавить Высокий урон, низкий урон
                
                //Частота
                if (towerViewModel.Parameters.TryGetValue(TowerParameterType.Speed, out var speed))
                    BaseParameters.Add(TowerParameterType.Speed, speed.Value);
                
                //Крит, Замедление, Оглушение, Здоровье, Перезарядка  если кол-во иконок меньше 3х
                if (BaseParameters.Count < 3 
                    && towerViewModel.Parameters.TryGetValue(TowerParameterType.Critical, out var critical))
                    BaseParameters.Add(TowerParameterType.Critical, critical.Value);
                
                if (BaseParameters.Count < 3 
                    && towerViewModel.Parameters.TryGetValue(TowerParameterType.SlowingDown, out var slow))
                    BaseParameters.Add(TowerParameterType.SlowingDown, slow.Value);
                if (BaseParameters.Count < 3 
                    && towerViewModel.Parameters.TryGetValue(TowerParameterType.Health, out var health))
                    BaseParameters.Add(TowerParameterType.Health, health.Value);
                //TODO Добавить Оглушение, Перезарядка


                
                UpdateInfoBackgroundTower.OnNext(true);
                NewPositionTowerInfo();
                ShowInfoTower.OnNext(true);
            }
        }

        
    }
}