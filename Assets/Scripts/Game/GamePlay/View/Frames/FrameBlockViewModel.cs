using System;
using System.Collections.Generic;
using System.Linq;
using Game.GamePlay.Services;
using Game.GamePlay.View.Grounds;
using Game.GamePlay.View.Roads;
using Game.GamePlay.View.Towers;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Frames
{
    public class FrameBlockViewModel : IDisposable
    {
        private readonly PlacementService _placementService;
        public ReactiveProperty<Vector2Int> Position { get; set; }
        public ReactiveProperty<bool> StartRemoveFlag = new(false);
        public ReactiveProperty<bool> FinishRemoveFlag = new(false);

        public ReactiveProperty<bool> Enable;
        public List<IMovingEntityViewModel> EntityViewModels = new();
        public ReactiveProperty<int> Rotate;
        public FrameType TypeElements;
        public ReactiveProperty<bool> IsSelected;

        public FrameBlockViewModel(Vector2Int position, PlacementService placementService)
        {
            _placementService = placementService;
            Position = new ReactiveProperty<Vector2Int>(position);
            Enable = new ReactiveProperty<bool>(true);
            Rotate = new ReactiveProperty<int>(0);
            IsSelected = new ReactiveProperty<bool>(false);
        }
        
        public void AddItem(IMovingEntityViewModel entityViewModel)
        {
            EntityViewModels.Add(entityViewModel);
            if (entityViewModel is TowerViewModel) TypeElements = FrameType.Tower;
            if (entityViewModel is RoadViewModel) TypeElements = FrameType.Road;
            if (entityViewModel is GroundFrameViewModel) TypeElements = FrameType.Ground;
        }

        public void MoveFrame(Vector2Int position)
        {
            Position.Value = position;
        }

        public bool IsRotate()
        {
            return IsRoad();
        }

        public void RotateFrame(int turn = 1)
        {
            if (IsRoad())
            {
                Rotate.Value += turn;
            }
        }

        public int GetRotateValue()
        {
            return Rotate.CurrentValue % 4; //от 0 до 4
        }

        public bool IsTower()
        {
            return TypeElements == FrameType.Tower;
        }
        
        public bool IsRoad()
        {
            return TypeElements == FrameType.Road;
        }
        public bool IsGround()
        {
            return TypeElements == FrameType.Ground;
        }

        public TowerViewModel GetTower()
        {
            return (TowerViewModel)EntityViewModels[0];
        }

        public List<RoadViewModel> GetRoads()
        {
            return EntityViewModels.Cast<RoadViewModel>().ToList();
        }

        public List<Vector2Int> GetGrounds()
        {
            return EntityViewModels.Select(entityViewModel => entityViewModel.GetPosition()).ToList();
        }

        public void Selected(bool value)
        {
            IsSelected.Value = value;
        }

        public void Dispose()
        {
            Enable?.Dispose();
            Rotate?.Dispose();
            IsSelected?.Dispose();
            Position?.Dispose();
        }

        public int GetCountFrames()
        {
            return IsGround() ? 1 : EntityViewModels.Count;
        }

        public ReactiveProperty<bool> StartRemove()
        {
            FinishRemoveFlag.Value = false;
            StartRemoveFlag.OnNext(true);

            return FinishRemoveFlag;
        }

        public void RotateTower()
        {
            if (!IsTower()) return;
            var directionTower = _placementService.GetDirectionTower(Position.CurrentValue);
            if (directionTower != Vector2Int.zero) GetTower().SetDirection(directionTower);
        }
    }
}