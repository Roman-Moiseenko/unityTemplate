using R3;
using UnityEngine;

namespace Game.GamePlay.View.Grounds
{
    public class GroundFrameViewModel : IMovingEntityViewModel
    {
        public ReactiveProperty<Vector2Int> Position { get; set; }

        public ReactiveProperty<bool> Enabled;

        public GroundFrameViewModel(Vector2Int position)
        {
            Position = new ReactiveProperty<Vector2Int>(position);
            Enabled = new ReactiveProperty<bool>(true);
        }
        public void SetPosition(Vector2Int position)
        {
            Position.Value = position;
        }

        public Vector2Int GetPosition()
        {
            return Position.CurrentValue;
        }
    }
}