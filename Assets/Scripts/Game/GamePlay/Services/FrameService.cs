using System.Collections;
using System.Collections.Generic;
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
        private readonly ObservableList<FrameBlock> _framesBlock = new();
        public IObservableCollection<FrameBlock> FramesBlock =>
            _framesBlock;
        public ISynchronizedView<FrameViewModel, FrameViewModel> ItemsView { get; set; }

        //public 

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
        }

        public void MoveFrame(Vector2Int position)
        {
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
        }

        public void SelectedFrame()
        {
            _frameBlock.Selected(true);
        }
        
        public void UnSelectedFrame()
        {
            _frameBlock.Selected(false);
        }
        public void RotateFrame()
        {
            if (_frameBlock.IsRotate)
            {
                _frameBlock.Rotate();
            }
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
            var frameViewModel = new FrameViewModel(position, _gameplayState.CreateEntityID());
            _frameBlock = new FrameBlockTower(position, towerViewModel, frameViewModel);
            _frameBlock.Enable.Value = _placementService.CheckPlacementTower(position, towerEntityId);
            _framesBlock.Add(_frameBlock);
        }

        public void RemoveFrame()
        {
            _framesBlock.Remove(_frameBlock);
            _frameBlock?.Dispose();
        }

        public bool IsPosition(Vector2Int position)
        {
            if (_frameBlock == null) return false;
            
            return _frameBlock.IsPosition(position);
        }

        public void CreateFrameRoad(Vector2Int position, string configId)
        {

            Debug.Log("CreateFrameRoad  configId = " + configId);
            List<RoadViewModel> list = new();
            _frameBlock = new FrameBlockRoad(position);
            switch (configId)
            {
                case "0":
                    list.Add(TemplateCreateRoad(position, true, 0)); // |-
                    break;
                
                case "1":
                    list.Add(TemplateCreateRoad(position, false, 0)); // |
                    break;
                //
                case "2":
                    list.Add(TemplateCreateRoad(position, false, 0)); // |
                    list.Add(TemplateCreateRoad(position, false, 0, 1)); // |
                    break;
                case "3":
                    list.Add(TemplateCreateRoad(position, false, 1)); // |
                    list.Add(TemplateCreateRoad(position, true, 3, 1)); // |_
                    break;
                case "4":
                    list.Add(TemplateCreateRoad(position, false, 1)); //  |
                    list.Add(TemplateCreateRoad(position, true, 2, 1)); // _|
                    break;
                case "5":
                    list.Add(TemplateCreateRoad(position, true, 1)); // -|
                    list.Add(TemplateCreateRoad(position, true, 3, 1)); //  |_
                    break;
                case "6":
                    list.Add(TemplateCreateRoad(position, true, 0)); //  |-
                    list.Add(TemplateCreateRoad(position, true, 2, 1)); // _|
                    break;
                case "7":
                    list.Add(TemplateCreateRoad(position, true, 1)); // -|
                    list.Add(TemplateCreateRoad(position, true, 2, 1)); // _|
                    break;
                case "8":
                    list.Add(TemplateCreateRoad(position, false, 1)); // |
                    list.Add(TemplateCreateRoad(position, true, 2, 1)); // _|
                    list.Add(TemplateCreateRoad(position, false, 0, 2)); // --
                    break;
                
            }

            foreach (var roadViewModel in list)
            {
                _frameBlock.As<FrameBlockRoad>().AddItem(
                    roadViewModel, 
                    new FrameViewModel(roadViewModel.Position.CurrentValue, _gameplayState.CreateEntityID()));
            }
            
            _frameBlock.Enable.Value = _placementService.CheckPlacementRoad(position, _frameBlock.As<FrameBlockRoad>().GetRoadIds());
            Debug.Log(JsonConvert.SerializeObject(_frameBlock, Formatting.Indented));
            _framesBlock.Add(_frameBlock);
            
        }

        public List<RoadViewModel> GetRoads()
        {
            return _frameBlock.As<FrameBlockRoad>().RoadViewModels;
        }
        
        
        
        
        private RoadViewModel TemplateCreateRoad(Vector2Int position, bool isTurn, int rotate, int indexOf = 0)
        {
          //  Vector2Int delta = new Vector2Int(0, 0);;
            Vector2Int delta = indexOf switch
            {
                1 => new Vector2Int(0, 1),
                2 => new Vector2Int(1, 1),
                _ => new Vector2Int(0, 0)
            };

            var roadEntity = new RoadEntity(new RoadEntityData
            {
                UniqueId = _gameplayState.CreateEntityID(),
                Position = position + delta,
                ConfigId = "Road",
                Rotate = rotate,
                IsTurn = isTurn,
            });
            
            return new RoadViewModel(roadEntity, _roadsService);
            
        }
    }
}