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

        public HeroViewModel(HeroEntity heroEntity, GameplayStateProxy gameplayState)
        {
            Debug.Log($"HeroViewModel {heroEntity.ConfigId}");
            _heroEntity = heroEntity;
            
            
            
        }
    }
}