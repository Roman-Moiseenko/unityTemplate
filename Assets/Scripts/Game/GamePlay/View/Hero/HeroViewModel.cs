using Game.State.Gameplay;
using Game.State.Maps.Heroes;

namespace Game.GamePlay.View.Hero
{
    public class HeroViewModel
    {
        private HeroEntity _heroEntity;

        public HeroViewModel(HeroEntity heroEntity, GameplayStateProxy gameplayState)
        {
            _heroEntity = heroEntity;
            
            
            
        }
    }
}