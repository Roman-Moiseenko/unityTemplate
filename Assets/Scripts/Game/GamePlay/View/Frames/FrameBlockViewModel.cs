using Game.State.Entities;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Frames
{
    public class FrameBlockViewModel
    {
        public ReadOnlyReactiveProperty<Vector2Int> Position { get; }
        public ReactiveProperty<bool> Enable;
        public Transform ParentTransform;
        public bool IsRotate;

        public EntityType EntityType;

        public FrameBlockViewModel(Vector2Int position)
        {
            Position = new ReactiveProperty<Vector2Int>(position);
            IsRotate = false;
            Enable = new ReactiveProperty<bool>(true);
        }
    }
}