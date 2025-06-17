using R3;
using UnityEngine;

namespace Game.GamePlay.View.Frames
{
    public class FrameViewModel
    {
        public ReadOnlyReactiveProperty<Vector2Int> Position { get; }
        public ReactiveProperty<bool> Enable;

        public FrameViewModel()
        {
            Enable = new ReactiveProperty<bool>();
            Position = new ReactiveProperty<Vector2Int>(new Vector2Int(0, 0));
        }
    }
}