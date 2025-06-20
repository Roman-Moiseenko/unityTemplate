using System;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Frames
{
    public class FrameViewModel
    {
        public ReactiveProperty<Vector2Int> Position { get; set; }
        public ReactiveProperty<bool> Enable;
        public ReactiveProperty<bool> IsSelected;
        public int EntityId; //Для совместимости

        public FrameViewModel(Vector2Int position, int entityId)
        {
            EntityId = entityId;
            Enable = new ReactiveProperty<bool>(true);
            Position = new ReactiveProperty<Vector2Int>(position);
            IsSelected = new ReactiveProperty<bool>(false);
        }

        public void Selected(bool value = true)
        {
            IsSelected.Value = value;
        }
    }
}