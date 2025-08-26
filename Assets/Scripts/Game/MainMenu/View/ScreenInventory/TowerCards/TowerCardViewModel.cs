using DI;
using Game.MainMenu.Services;
using Game.Settings.Gameplay.Entities.Tower;
using Game.State.Inventory;
using Game.State.Inventory.TowerCards;
using R3;
using UnityEngine;

namespace Game.MainMenu.View.ScreenInventory.TowerCards
{
    public class TowerCardViewModel
    {
        public TowerCard TowerCard => _towerCardEntity;
        public readonly TowerSettings TowerSettings;
        public string ConfigId => _towerCardEntity.ConfigId;
        public ReadOnlyReactiveProperty<TypeEpicCard> EpicLevel => _towerCardEntity.EpicLevel;
        public ReadOnlyReactiveProperty<int> Level => _towerCardEntity.Level;
        public int IdTowerCard => _towerCardEntity.UniqueId;
        
        private readonly TowerCard _towerCardEntity;
        private readonly TowerCardPlanService _planService;
        private readonly DIContainer _container;
        
        public TowerCardViewModel(
            TowerCard towerCardEntity, 
            TowerSettings towerSettings, 
            TowerCardPlanService planService,
            DIContainer container
            )
        {
            _towerCardEntity = towerCardEntity;
            TowerSettings = towerSettings;
            _planService = planService;
            _container = container;
        }
        
        public void RequestOpenPopupTowerCard()
        {
            _container.Resolve<Subject<TowerCardViewModel>>().OnNext(this);
        }
    }
}