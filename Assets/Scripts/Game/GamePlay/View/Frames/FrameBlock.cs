using System;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Frames
{
    public abstract class FrameBlock : IDisposable
    {
        public ReactiveProperty<Vector2Int> Position { get; set; }
        public ReactiveProperty<bool> Enable;
        public bool IsRotate;

        public FrameBlock(Vector2Int position)
        {
            Enable = new ReactiveProperty<bool>(true);
            Position = new ReactiveProperty<Vector2Int>(position);
        }

        public Observable<bool> Enabled()
        {
            return Enable;
        }

        public void Dispose()
        {
            Enable?.Dispose();
            Position?.Dispose();
        }
        public T As<T>() where T : FrameBlock
        {
            return (T)this;
        }

        public virtual bool FrameIs(FrameType frameType)
        {
            throw new Exception("Не указан тип");
        }
        
        public virtual void Move(Vector2Int position) {}
        public virtual void Rotate() {}
    }
}