using System;
using DI;
using Game.Common;
using Game.GamePlay.Classes;
using Game.GamePlay.View.Towers;
using Game.State.Maps.Towers;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.UI.PanelGateWave.InfoTower
{
    public class InfoTowerViewModel
    {
        public ReactiveProperty<bool> ShowInfoTower = new(false);
        public ReactiveProperty<Vector3> PositionInfoTower = new(Vector3.zero);
        private readonly GameplayCamera _cameraService;
        
        public ObservableDictionary<TowerParameterType, float> BaseParameters = new(); 
        public ObservableDictionary<TowerParameterType, float> UpgradeParameters = new();
        
        private Vector2Int _towerPrevious = Vector2Int.zero;
        public TowerViewModel TowerViewModel;
        
        public InfoTowerViewModel(DIContainer container)
        {
            var entityClick = container.Resolve<Subject<Unit>>(AppConstants.CLICK_WORLD_ENTITY);
            var towerClick = container.Resolve<Subject<TowerViewModel>>();
            var positionCamera = container.Resolve<Subject<Unit>>(AppConstants.CAMERA_MOVING);
            _cameraService = container.Resolve<GameplayCamera>();
            entityClick.Subscribe(_ =>
            {
                if (ShowInfoTower.CurrentValue) ShowInfoTower.OnNext(false);
            });
            //Событие при клике по башне
            towerClick.Subscribe(towerViewModel =>
            {
                if (_towerPrevious == towerViewModel.Position.CurrentValue)
                {
                    if (ShowInfoTower.CurrentValue) ShowInfoTower.OnNext(false);
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
            });
            
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
    }
}