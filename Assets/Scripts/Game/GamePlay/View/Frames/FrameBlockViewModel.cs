using System;
using System.Collections.Generic;
using System.Linq;
using Game.GamePlay.View.Grounds;
using Game.GamePlay.View.Roads;
using Game.GamePlay.View.Towers;
using Game.State.Entities;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Frames
{
    public class FrameBlockViewModel : IDisposable
    {
        public ReactiveProperty<Vector2Int> Position { get; set; }
        public ReactiveProperty<bool> Enable;
        public List<IMovingEntityViewModel> EntityViewModels = new();
        public ReactiveProperty<int> Rotate;
        private FrameType _type;
        public ReactiveProperty<bool> IsSelected;

        public FrameBlockViewModel(Vector2Int position)
        {
            Position = new ReactiveProperty<Vector2Int>(position);
            Enable = new ReactiveProperty<bool>(true);
            Rotate = new ReactiveProperty<int>(0);
            IsSelected = new ReactiveProperty<bool>(false);
        }
        
        public void AddItem(IMovingEntityViewModel entityViewModel)
        {
            EntityViewModels.Add(entityViewModel);
            if (entityViewModel is TowerViewModel) _type = FrameType.Tower;
            if (entityViewModel is RoadViewModel) _type = FrameType.Road;
            if (entityViewModel is GroundFlagViewModel) _type = FrameType.Ground;
        }

        public void MoveFrame(Vector2Int position)
        {
            //var delta = position - EntityViewModels[0].GetPosition();
            Position.Value = position;
        }

        public bool IsRotate()
        {
            return IsRoad();
        }
        public void RotateFrame()
        {
            if (IsRoad())
            {
                Rotate.Value++;
            }
        }

        public int GetRotateValue()
        {
            return Rotate.CurrentValue % 4; //от 0 до 4
        }

        public bool IsTower()
        {
            return _type == FrameType.Tower;
        }
        
        public bool IsRoad()
        {
            return _type == FrameType.Road;
        }
        public bool IsGround()
        {
            return _type == FrameType.Ground;
        }

        public int GetTowerId()
        {
            return ((TowerViewModel)EntityViewModels[0]).TowerEntityId;
        }

        public List<RoadViewModel> GetRoads()
        {
            return EntityViewModels.Cast<RoadViewModel>().ToList();
        }

        public List<Vector2Int> GetGrounds()
        {
            return EntityViewModels.Select(entityViewModel => entityViewModel.GetPosition()).ToList();
        }

        public void Selected(bool value = true)
        {
            IsSelected.Value = value;
        }

        public bool IsPosition(Vector2Int position)
        {
            if (IsTower() || IsGround())
                return Position.CurrentValue == position;
            if (IsRoad())
            {
                //TODO Определяем координаты 
                var p0 = EntityViewModels[0].GetPosition();
                foreach (var entityViewModel in EntityViewModels)
                {
                    Debug.Log("position = " + entityViewModel.GetPosition().x + " " + entityViewModel.GetPosition().y);
                }
            }
            return false;
            //return Position.CurrentValue == position;
            //return EntityViewModels.Any(entityViewModel => entityViewModel is not GroundFrameViewModel && entityViewModel.GetPosition() == position);
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
    }
}