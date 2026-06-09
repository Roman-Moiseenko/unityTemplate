using System;
using System.Collections.Generic;
using Game.State.Common;
using Game.State.Parameters;
using R3;
using UnityEngine;

namespace Game.State.Maps.Heroes
{
    public class HeroEntity : IDisposable
    {
        public HeroEntityData Origin { get; }
        public int UniqueId => Origin.UniqueId;
        public string ConfigId => Origin.ConfigId;
        public TypeEpic EpicLevel => Origin.EpicLevel;
        public TypeTarget TypeTarget => Origin.TypeTarget;
        public TypeDefence Defence => Origin.Defence;
        public bool IsSingleTarget => Origin.IsSingleTarget;

        
        public readonly ReactiveProperty<int> GameplayLevel;
        public readonly ReactiveProperty<Vector2> Position;
        public readonly ReactiveProperty<Vector2Int> Placement; //Округленный Position
        public Dictionary<ParameterType, ParameterData> Parameters = new();
        
        private DisposableBag _disposables;
        
        public HeroEntity(HeroEntityData heroEntityData)
        {
            Origin = heroEntityData;
            
            Position = new ReactiveProperty<Vector2>(heroEntityData.Placement);
            //Позицию меняет сервис при движении при атаке
/*            Position
                .Subscribe(newPosition => heroEntityData.Position = newPosition)
                .AddTo(ref _disposables); //При изменении позиции Position.Value меняем в данных
*/
            Placement = new ReactiveProperty<Vector2Int>(heroEntityData.Placement); //Базовое размещение, к которому возвращается Position в конце волны
            Placement.Subscribe(v => heroEntityData.Placement = v).AddTo(ref _disposables);
            
            Debug.Log("Сущность героя");
            
            GameplayLevel = new ReactiveProperty<int>(heroEntityData.GameplayLevel);
            GameplayLevel.Subscribe(newLevel =>
            {
                heroEntityData.GameplayLevel = newLevel;
            }).AddTo(ref _disposables);
        }
        
        
        public void Dispose()
        {
            GameplayLevel?.Dispose();
            Position?.Dispose();
            Placement?.Dispose();
            _disposables.Dispose();
        }
        
    }
}