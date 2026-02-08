using System;
using System.Collections.Generic;
using DI;
using Game.Common;
using Game.GamePlay.Classes;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.TowerStates;
using Game.GamePlay.View.Towers;
using Game.Settings;
using Game.Settings.Gameplay.Entities.Tower;
using Game.State.Inventory;
using Game.State.Maps.Mobs;
using Game.State.Maps.Towers;
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
        
        public ObservableDictionary<TowerParameterType, float> BaseParameters = new(); 
        public ObservableDictionary<TowerParameterType, float> UpgradeParameters = new();
        
        private Vector2Int _towerPrevious = Vector2Int.zero;
        public TowerViewModel TowerViewModel;

        public string NameTower;
        public TypeEpicCard EpicLevel;
        public int Level;
        public MobDefence Defence;
        
        private readonly GameSettings _gameSettings;
        private readonly List<TowerSettings> _settingsTowers;

        public InfoTowerViewModel(DIContainer container)
        {
            var positionCamera = container.Resolve<Subject<Unit>>(AppConstants.CAMERA_MOVING);
            var fsmTower = container.Resolve<FsmTower>();
            var gameSettings = container.Resolve<ISettingsProvider>().GameSettings;
            _settingsTowers = gameSettings.TowersSettings.AllTowers;
                
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
            var p = new Vector3(_towerPrevious.x, 0, _towerPrevious.y);
            var v= _cameraService.Camera.WorldToScreenPoint(p);
            v.z = 0;
            v.y += 100;
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
                UpgradeParameters.Clear();
                    
                foreach (var parameterData in towerViewModel.Parameters)
                {
                    BaseParameters.Add(parameterData.Key, parameterData.Value.Value);
                }
                UpdateInfoBackgroundTower.OnNext(true);
                NewPositionTowerInfo();
                ShowInfoTower.OnNext(true);
            }
        }

        
    }
}