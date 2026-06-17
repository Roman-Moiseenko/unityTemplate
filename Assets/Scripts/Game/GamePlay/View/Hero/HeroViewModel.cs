using Game.Settings.Gameplay.Entities.Heroes;
using Game.State.Gameplay;
using Game.State.Maps.Heroes;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Hero
{
    public class HeroViewModel
    {
        private readonly HeroEntity _heroEntity;
        public string ConfigId => _heroEntity.ConfigId;
        public ReactiveProperty<Vector2> Position => _heroEntity.Position;
        
        public ReactiveProperty<int> GameplayLevel => _heroEntity.GameplayLevel;

        public HeroViewModel(HeroEntity heroEntity, HeroSettings heroSettings)
        {
            Debug.Log($"HeroViewModel {heroEntity.ConfigId}");
            _heroEntity = heroEntity;
            
            
        }
    }
}