using R3;
using UnityEngine;

namespace Game.GamePlay.View.Frames
{
    public class FrameViewModel
    {
        public ReadOnlyReactiveProperty<Vector2Int> Position { get; }
        public ReactiveProperty<bool> Enable;
        public Transform ParentTransform;
        public int EntityId;

        public FrameViewModel(Vector2Int position, int entityId)
        {
          //  ParentTransform = parentTransform;
            EntityId = entityId;
            Enable = new ReactiveProperty<bool>(true);
            Position = new ReactiveProperty<Vector2Int>(position);
        }
    }
}