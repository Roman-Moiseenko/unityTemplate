using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.GamePlay.View;
using Game.GamePlay.View.AttackAreas;
using Game.GamePlay.View.Frames;
using Game.GamePlay.View.Grounds;
using Game.GamePlay.View.Roads;
using Game.GamePlay.View.Towers;
using Game.Settings.Gameplay.Entities.Tower;
using Game.State.Maps.Roads;
using Game.State.Maps.Towers;
using Game.State.Root;
using MVVM.CMD;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using Unity.Mathematics.Geometry;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.GamePlay.Services
{
    public class FrameService
    {
        private readonly GameplayStateProxy _gameplayState;
        private readonly PlacementService _placementService;
        private readonly TowersService _towerService;
        private readonly RoadsService _roadsService;
        
        private FrameBlockViewModel _viewModel;
        private readonly ObservableList<FrameBlockViewModel> _viewModels = new();
        public IObservableCollection<FrameBlockViewModel> ViewModels => _viewModels;
        private Dictionary<string, bool> _towerOnRoadMap = new();
        private Dictionary<string, bool> _towerParametersMap = new();
        
        private Dictionary<int, Vector2Int> matrixRoads = new();
        private AttackAreaViewModel _areaViewModel;

        public FrameService(
            GameplayStateProxy gameplayState,
            PlacementService placementService,
            TowersService towerService,
            RoadsService roadsService,
            TowersSettings towersSettings
            )
        {
            _gameplayState = gameplayState;
            _placementService = placementService;
            _towerService = towerService;
            _roadsService = roadsService;
            matrixRoads.Add(0, new Vector2Int(0, 0));
            matrixRoads.Add(1, new Vector2Int(0, 1));
            matrixRoads.Add(2, new Vector2Int(1, 1));
            matrixRoads.Add(3, new Vector2Int(1, 0));

            foreach (var towerSettings in towersSettings.AllTowers)
            {
                _towerOnRoadMap.Add(towerSettings.ConfigId, towerSettings.OnRoad);
            }
        }

        public void MoveFrame(Vector2Int position)
        {
            _viewModel.MoveFrame(position);

            if (_viewModel.IsTower())
            {
                _viewModel.Enable.Value = _placementService.CheckPlacementTower(position,
                    _viewModel.GetTower().TowerEntityId, _viewModel.GetTower().IsOnRoad);
                if (_areaViewModel != null)
                {
                    _areaViewModel.SetPosition(position);
                    if (!_viewModel.Enable.Value)
                    {
                        _areaViewModel.Hide();
                    }
                    else
                    {
                        _areaViewModel.Restore();
                    }
                };
            }

            if (_viewModel.IsRoad())
                _viewModel.Enable.Value = _placementService.CheckPlacementRoad(GetRoads());

            if (_viewModel.IsGround())
            {
                var centerFrame = _viewModel.EntityViewModels.Cast<GroundFrameViewModel>().ToList()[0]; 
                if (!_placementService.CheckPlacementFrameGround(centerFrame.GetPosition() + _viewModel.Position.CurrentValue))
                {  //Центральный фрейм не на земле
                    foreach (var item in _viewModel.EntityViewModels.Cast<GroundFrameViewModel>())
                    {
                        item.Enabled.Value = false;
                    }
                        
                    _viewModel.Enable.Value = false;
                }
                else
                {
                    var enableItems = false;
                    foreach (var item in _viewModel.EntityViewModels.Cast<GroundFrameViewModel>())
                    {
                        item.Enabled.Value = !_placementService.CheckPlacementFrameGround(item.GetPosition() + _viewModel.Position.CurrentValue);
                        if (item.Enabled.Value) enableItems = true;
                    }
                    _viewModel.Enable.Value = enableItems;
                }
            }
        }

        public void SelectedFrame()
        {
            _viewModel.Selected(true);
        }
        
        public void UnSelectedFrame()
        {
            _viewModel.Selected(false);
        }
        public void RotateFrame()
        {
            if (_viewModel.IsRoad())
            {
                _viewModel.RotateFrame();
                _viewModel.Enable.Value = _placementService.CheckPlacementRoad(GetRoads());    
            }
        }

        public void CreateFrameTower(Vector2Int position, int level, string configId, AttackAreaViewModel areaViewModel)
        {
            var towerEntityId = _gameplayState.CreateEntityID();
            var towerEntity = new TowerEntity(new TowerEntityData
            {
                UniqueId = towerEntityId,
                Position = position,
                Level = level,
                ConfigId = configId,
                IsOnRoad = _towerOnRoadMap[configId],
            });
            towerEntity.Parameters = _towerService.TowerParametersMap[configId];
            var towerViewModel = new TowerViewModel(towerEntity, null, _towerService);
            _areaViewModel = areaViewModel;
            _areaViewModel.SetStartPosition(towerEntity.Position.Value);
            _areaViewModel.SetRadius(towerViewModel.GetRadius());
            
            _viewModel = new FrameBlockViewModel(position);
            _viewModel.AddItem(towerViewModel);
            _viewModels.Add(_viewModel);
            _viewModel.Enable.Value = _placementService.CheckPlacementTower(position, towerEntityId, towerEntity.IsOnRoad);
        }

        public void RemoveFrame()
        {
            if (_areaViewModel != null)
            {
                _areaViewModel.Hide();
                _areaViewModel = null;
            }
            _viewModels.Remove(_viewModel);
            _viewModel?.Dispose();
        }

        public bool IsPosition(Vector2Int position)
        {
            if (_viewModel == null) return false;
            
            if (_viewModel.IsTower() || _viewModel.IsGround())
                return _viewModel.Position.CurrentValue == position;
            if (_viewModel.IsRoad())
            {
                var rotateIndex = _viewModel.GetRotateValue();
                var realPosition = _viewModel.Position.CurrentValue - matrixRoads[rotateIndex];
                foreach (var entityViewModel in _viewModel.EntityViewModels)
                {
                    if (position == entityViewModel.GetPosition() + realPosition) return true;
                }
            }
            return false;
        }

        /**
         * Создаем блок дороги в зависимости от конфигурации
         */
        public void CreateFrameRoad(Vector2Int position, string configId)
        {
            _viewModel = new FrameBlockViewModel(position);
            switch (configId)
            {
                case "0":
                    _viewModel.AddItem(TemplateCreateRoad(true, 0));
                    break;
                
                case "1":
                    _viewModel.AddItem(TemplateCreateRoad(false, 0)); // |
                    break;
                //
                case "2":
                    _viewModel.AddItem(TemplateCreateRoad(false, 1)); // |
                    _viewModel.AddItem(TemplateCreateRoad(false, 1, 1)); // |
                    break;
                case "3":
                    _viewModel.AddItem(TemplateCreateRoad(false, 1)); //          |
                    _viewModel.AddItem(TemplateCreateRoad(true, 3, 1)); // |_
                    break;
                case "4":
                    _viewModel.AddItem(TemplateCreateRoad(false, 1)); //           |
                    _viewModel.AddItem(TemplateCreateRoad(true, 2, 1)); // _|
                    break;
                case "5":
                    _viewModel.AddItem(TemplateCreateRoad(true, 1)); //          -|
                    _viewModel.AddItem(TemplateCreateRoad(true, 3, 1)); //  |_
                    break;
                case "6":
                    _viewModel.AddItem(TemplateCreateRoad(true, 0)); //            |-
                    _viewModel.AddItem(TemplateCreateRoad(true, 2, 1)); //  _|
                    break;
                case "7":
                    _viewModel.AddItem(TemplateCreateRoad(true, 1)); //          -|
                    _viewModel.AddItem(TemplateCreateRoad(true, 2, 1)); // _|
                    break;
                case "8":
                    _viewModel.AddItem(TemplateCreateRoad(false, 1)); // |
                    _viewModel.AddItem(TemplateCreateRoad(true, 2, 1)); // _|
                    _viewModel.AddItem(TemplateCreateRoad(false, 2, 2)); // --
                    break;
            }

            _viewModel.Enable.Value = _placementService.CheckPlacementRoad(GetRoads());
            _viewModels.Add(_viewModel);
            
        }

        /**
         * Копия списка дорог во фрейме типа <RoadEntityData>
         * для внутренней обработки - проверка присоединения, проверка размещения
         */
        private List<RoadEntityData> GetRoads()
        {

            List<RoadEntityData> result = new();
            
            var roads = _viewModel.EntityViewModels.Cast<RoadViewModel>().ToList();
            
            var i = 0;
            var rotateIndex = _viewModel.GetRotateValue();
            var realPosition = _viewModel.Position.CurrentValue - matrixRoads[rotateIndex];
            
            foreach (var road in roads)
            {
                result.Add(new RoadEntityData
                {
                    ConfigId = road.ConfigId,
                    UniqueId = road.RoadEntityId,
                    IsTurn = road.IsTurn,
                    Position = realPosition + matrixRoads[(i + rotateIndex) % 4],
                    Rotate = road.Rotate.Value + rotateIndex
                });
                i++;
            }
            
            return result;
        }

        /**
         * Список дорог для передачи в строительство,
         * при присоединении к последнему road, делаем реверс 
         */
        public List<RoadEntityData> GetRoadsForBuild()
        {
            var list = GetRoads();
            if (_placementService.IsLastPontForWay(list))
            {
                list.Reverse();
            }
            return list;
        }
        /**
         * Создаем RoadViewModel по шаблону, для генерации видов блоков дорог
         */
        private RoadViewModel TemplateCreateRoad(bool isTurn, int rotate, int indexOf = 0)
        {
            var roadEntity = new RoadEntity(new RoadEntityData
            {
                UniqueId = _gameplayState.CreateEntityID(),
                Position = matrixRoads[indexOf],
                ConfigId = "Road",
                Rotate = rotate,
                IsTurn = isTurn,
            });
            
            return new RoadViewModel(roadEntity, _roadsService);
            
        }

        /**
         * Проверка, к какому пути присоединили фрейм с дорогой
         */
        public bool IsMainPath()
        {
            //Проверка на главный путь
            return _placementService.IsMainWay(GetRoads());
        }

        public void CreateFrameGround(Vector2Int position)
        {
           // Debug.Log("CreateFrameGround " + position);
            _viewModel = new FrameBlockViewModel(position);
            //_viewModel.AddItem(towerViewModel);
            _viewModel.AddItem(new GroundFrameViewModel(new Vector2Int(0, 0)));
            _viewModel.AddItem(new GroundFrameViewModel(new Vector2Int(1, 0)));
            _viewModel.AddItem(new GroundFrameViewModel(new Vector2Int(1, 1)));
            _viewModel.AddItem(new GroundFrameViewModel(new Vector2Int(0, 1)));
            _viewModel.AddItem(new GroundFrameViewModel(new Vector2Int(-1, 1)));
            _viewModel.AddItem(new GroundFrameViewModel(new Vector2Int(-1, 0)));
            _viewModel.AddItem(new GroundFrameViewModel(new Vector2Int(-1, -1)));
            _viewModel.AddItem(new GroundFrameViewModel(new Vector2Int(0, -1)));
            _viewModel.AddItem(new GroundFrameViewModel(new Vector2Int(1, -1)));
            _viewModel.AddItem(new GroundFrameViewModel(new Vector2Int(2, 0)));
            _viewModel.AddItem(new GroundFrameViewModel(new Vector2Int(-2, 0)));
            _viewModel.AddItem(new GroundFrameViewModel(new Vector2Int(0, 2)));
            _viewModel.AddItem(new GroundFrameViewModel(new Vector2Int(0, -2)));
            
            
            _viewModels.Add(_viewModel);
            //_viewModel.Enable.Value = _placementService.CheckPlacementGround(position);
        }

        public IEnumerable<Vector2Int> GetGrounds()
        {
            return _viewModel.
                EntityViewModels.
                Select(item => item.GetPosition() + _viewModel.Position.CurrentValue).
                ToList();
        }
    }
}