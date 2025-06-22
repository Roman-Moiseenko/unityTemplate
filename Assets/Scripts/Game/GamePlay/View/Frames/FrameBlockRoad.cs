using System.Collections.Generic;
using System.Linq;
using Game.GamePlay.View.Roads;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Frames
{
    public class FrameBlockRoad : FrameBlock
    {
        public List<RoadViewModel> RoadViewModels { get; } = new();
        public List<FrameViewModel> FrameViewModels { get; } = new();

        public FrameBlockRoad(Vector2Int position) : base(position)
        {
            IsRotate = true;
            Enable.Subscribe(newValue =>
            {
                foreach (var frameViewModel in FrameViewModels)
                {
                    frameViewModel.Enable.Value = newValue;
                }
            });
        }

        public void AddItem(RoadViewModel roadViewModel, FrameViewModel frameViewModel)
        {
            RoadViewModels.Add(roadViewModel);
            FrameViewModels.Add(frameViewModel);
        }
        
        public override bool FrameIs(FrameType frameType)
        {
            return FrameType.Road == frameType;
        }
        public override void Move(Vector2Int position)
        {
            var p0 = FrameViewModels[0].Position.Value;
            var delta = position - p0;
            foreach (var frameViewModel in FrameViewModels)
            {
                frameViewModel.Position.Value += delta;
            }
            
            foreach (var roadViewModel in RoadViewModels)
            {
                roadViewModel.Position.Value += delta;
            }
        }
        public override void Rotate()
        {
            //Обходим циклом и меняем координаты
            Debug.Log("Поворачиваем фрейм");
        }
        
        public override void Selected(bool value = true)
        {
            foreach (var frameViewModel in FrameViewModels)
            {
                frameViewModel.Selected(value);
            }
        }

        public override bool IsPosition(Vector2Int position)
        {
            foreach (var frameViewModel in FrameViewModels.Where(frameViewModel => frameViewModel.Position.Value == position))
            {
                return true;
            }
            return false;
        }

        public List<int> GetRoadIds()
        {
            List<int> ids = new();
            foreach (var roadViewModel in RoadViewModels)
            {
                ids.Add(roadViewModel.RoadEntityId);
            }

            return ids;
        }
    }
}