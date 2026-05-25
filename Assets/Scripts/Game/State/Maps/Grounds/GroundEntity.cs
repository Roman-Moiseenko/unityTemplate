using System;
using Cysharp.Threading.Tasks;
using Game.State.Entities;
using R3;
using UnityEngine;

namespace Game.State.Maps.Grounds
{
    public class GroundEntity : IDisposable
    {
        //       public GroundType GroundType;
        public GroundEntityData Origin { get; }
        public int UniqueId => Origin.UniqueId;
        public string ConfigId => Origin.ConfigId;
        
        public readonly ReactiveProperty<Vector2Int> Position;
        public readonly ReactiveProperty<bool> Enabled;
        private DisposableBag _disposables = new();

        public GroundEntity(GroundEntityData groundData)
        {
            //   GroundType = entityData.GroundType;
            Enabled = new ReactiveProperty<bool>(groundData.Enabled);
            Enabled.Subscribe(newValue => groundData.Enabled = newValue).AddTo(ref _disposables);
            Origin = groundData;
            Position = new ReactiveProperty<Vector2Int>(groundData.Position);
            Position.Subscribe(newPosition => groundData.Position = newPosition).AddTo(ref _disposables); 
        }

        public void Dispose()
        {
            Position?.Dispose();
            Enabled?.Dispose();
            _disposables.Dispose();
        }
    }
}