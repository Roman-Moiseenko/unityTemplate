using System;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Frames.SkillFrames
{
    
    public class FrameSkillViewModel : IDisposable
    {
        public ReactiveProperty<Vector2Int> Position { get; set; }
        public ReactiveProperty<bool> Enable;
        public string ConfigId;
        
        public FrameSkillViewModel(string configId)
        {
            ConfigId = configId;
            Position = new ReactiveProperty<Vector2Int>(Vector2Int.zero);
            Enable = new ReactiveProperty<bool>(false);
        }
        
        public void MoveFrame(Vector2Int position)
        {
            Position.Value = position;
        }

        public void Dispose()
        {
            Enable?.Dispose();
            Position?.Dispose();
        }
    }
    

}