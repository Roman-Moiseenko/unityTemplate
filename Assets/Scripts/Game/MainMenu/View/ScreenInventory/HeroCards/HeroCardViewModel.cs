using System;
using DI;
using Game.Settings.Gameplay.Entities.Heroes;
using Game.State.Inventory.HeroCards;
using R3;
using UnityEngine;

namespace Game.MainMenu.View.ScreenInventory.HeroCards
{
    public class HeroCardViewModel : IDisposable
    {
        private DisposableBag _disposables;
        private readonly HeroCard _heroCardEntity;
        public ReactiveProperty<bool> IsDeck = new(false);
        public string ConfigId => _heroCardEntity.ConfigId;
        
        public HeroCardViewModel(
            HeroCard heroCardEntity, 
            HeroSettings heroSettings, 
            DIContainer container
            )
        {
            _heroCardEntity = heroCardEntity;
//            Debug.Log("HeroCardViewModel создана " + IsDeck.Value);
 //           IsDeck.Subscribe(v => Debug.Log($"IsDeck = " + v)).AddTo(ref _disposables);
        }

        


        public void Dispose()
        {
            IsDeck?.Dispose();
            _disposables.Dispose();
        }
    }
}