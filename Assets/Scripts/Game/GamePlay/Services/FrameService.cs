using Game.GamePlay.View.Frames;
using Game.GamePlay.View.Towers;
using Game.State.Maps.Towers;
using Game.State.Root;
using MVVM.CMD;
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
        
        private FrameBlock _frameBlock;
        private readonly ObservableList<FrameBlock> _framesBlock = new();
        public IObservableCollection<FrameBlock> FramesBlock =>
            _framesBlock;
        public ISynchronizedView<FrameViewModel, FrameViewModel> ItemsView { get; set; }

        //public 

        public FrameService(
            GameplayStateProxy gameplayState,
            PlacementService placementService,
            TowersService towerService)
        {
            _gameplayState = gameplayState;
            _placementService = placementService;
            _towerService = towerService;
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
                //
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
            _frameBlock.Dispose();
        }

        public bool IsPosition(Vector2Int position)
        {
            if (_frameBlock == null) return false;
            
            return _frameBlock.IsPosition(position);
        }
    }
}