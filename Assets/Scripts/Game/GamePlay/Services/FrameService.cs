using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.GamePlay.View;
using Game.GamePlay.View.Frames;
using Game.GamePlay.View.Roads;
using Game.GamePlay.View.Towers;
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

        private FrameBlock _frameBlock;
        
        private FrameBlockViewModel _viewModel;
        private readonly ObservableList<FrameBlockViewModel> _viewModels = new();
        public IObservableCollection<FrameBlockViewModel> ViewModels => _viewModels;
        
        private readonly ObservableList<FrameBlock> _framesBlock = new();
        public IObservableCollection<FrameBlock> FramesBlock =>
            _framesBlock;

        //public 
    //    private Dictionary<int, Vector2Int> _rotations = new();
        private Dictionary<int, Vector2Int> matrixRoads = new();

        public FrameService(
            GameplayStateProxy gameplayState,
            PlacementService placementService,
            TowersService towerService,
            RoadsService roadsService
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
        }

        public void MoveFrame(Vector2Int position)
        {
            _viewModel.MoveFrame(position);
            
            if (_viewModel.IsTower())
                _viewModel.Enable.Value = _placementService.CheckPlacementTower(position, _viewModel.GetTowerId());
            if (_viewModel.IsRoad())
                _viewModel.Enable.Value = _placementService.CheckPlacementRoad(position, GetRoads());

            if (_viewModel.IsGround())
            {
                //TODO Получить список, где появится земля
                //_viewModel.SetEnabledGround(_placementService.CheckPlacementGround(position, _viewModel.GetGrounds()));
                //_viewModel.Enable.Value = ;
            }
            /*
            if (_frameBlock.FrameIs(FrameType.Tower))
            {
                var towerEntityId = _frameBlock.As<FrameBlockTower>().TowerViewModel.TowerEntityId;
                _frameBlock.Enable.Value = _placementService.CheckPlacementTower(position, towerEntityId);
            }

            if (_frameBlock.FrameIs(FrameType.Road))
            {
                //TODO Проверка на размещение
            }
            
            if (_frameBlock.FrameIs(FrameType.Ground))
            {
                //
            }
            
            _frameBlock.Move(position);
            */
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
            _viewModel.RotateFrame();
        }

        public void CreateFrameTower(Vector2Int position, int level, string configId)
        {
            var towerEntityId = _gameplayState.CreateEntityID();
            var towerEntity = new TowerEntity(new TowerEntityData
            {
                UniqueId = towerEntityId,
                Position = position,
                Level = level,
                ConfigId = configId
            });
            
            var towerViewModel = new TowerViewModel(towerEntity, null, _towerService);

            _viewModel = new FrameBlockViewModel(position);
            _viewModel.AddItem(towerViewModel);
            _viewModels.Add(_viewModel);
            _viewModel.Enable.Value = _placementService.CheckPlacementTower(position, towerEntityId);
        }

        public void RemoveFrame()
        {
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
                    _viewModel.AddItem(TemplateCreateRoad(false, 0)); // |
                    _viewModel.AddItem(TemplateCreateRoad(false, 0, 1)); // |
                    break;
                case "3":
                    _viewModel.AddItem(TemplateCreateRoad(false, 1)); // |
                    _viewModel.AddItem(TemplateCreateRoad(true, 3, 1)); // |_
                    break;
                case "4":
                    _viewModel.AddItem(TemplateCreateRoad(false, 1)); //  |
                    _viewModel.AddItem(TemplateCreateRoad(true, 2, 1)); // _|
                    break;
                case "5":
                    _viewModel.AddItem(TemplateCreateRoad(true, 1)); // -|
                    _viewModel.AddItem(TemplateCreateRoad(true, 3, 1)); //  |_
                    break;
                case "6":
                    _viewModel.AddItem(TemplateCreateRoad(true, 0)); //  |-
                    _viewModel.AddItem(TemplateCreateRoad(true, 2, 1)); // _|
                    break;
                case "7":
                    _viewModel.AddItem(TemplateCreateRoad(true, 1)); // -|
                    _viewModel.AddItem(TemplateCreateRoad(true, 2, 1)); // _|
                    break;
                case "8":
                    _viewModel.AddItem(TemplateCreateRoad(false, 1)); // |
                    _viewModel.AddItem(TemplateCreateRoad(true, 2, 1)); // _|
                    _viewModel.AddItem(TemplateCreateRoad(false, 0, 2)); // --
                    break;
            }

            _viewModel.Enable.Value = _placementService.CheckPlacementRoad(position, GetRoads());
            _viewModels.Add(_viewModel);
            
        }

        public List<RoadEntityData> GetRoads()
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
    }
}