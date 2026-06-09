using System;
using DI;
using Game.GamePlay.Root;
using Game.GamePlay.View.Hero;
using Game.State.Gameplay;
using Game.State.Maps.Heroes;
using Game.State.Research;
using R3;

namespace Game.GamePlay.Services
{
    public class HeroService : IDisposable
    {
        private readonly HeroEntity _heroEntity;
        private readonly GameplayBoosters _gameplayBoosters;
        
        public HeroViewModel HeroViewModel { get; } 
        
        private DisposableBag _disposables;

        public HeroService(DIContainer container,
            GameplayStateProxy gameplayState,
            GameplayEnterParams gameplayEnterParams)
        {
            _heroEntity = gameplayState.Hero;
            _gameplayBoosters = gameplayEnterParams.GameplayBoosters;
            HeroViewModel = new HeroViewModel(_heroEntity, gameplayState);
            //TODO Увеличить урон, скорость и HP от CastleResearch
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}