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
        
        private readonly TowerCard _towerCardEntity;
        public TowerCard TowerCard => _towerCardEntity;
        public readonly TowerSettings TowerSettings;
        public Vector2Int Position = Vector2Int.zero;
        public string ConfigId => _towerCardEntity.ConfigId;

        private readonly TowerCardService _service;
        private readonly DIContainer _container;

        private readonly MainMenuUIManager _uiManager;
        //  private TowerCard _towerCardEntity;

        public ReadOnlyReactiveProperty<TypeEpicCard> EpicLevel => _towerCardEntity.EpicLevel;
        public ReadOnlyReactiveProperty<int> Level => _towerCardEntity.Level;
        public int IdTowerCard => _towerCardEntity.UniqueId;


        public TowerCardViewModel(
            TowerCard towerCardEntity, 
            TowerSettings towerSettings, 
            TowerCardService service,
            DIContainer container
            )
        {
            _towerCardEntity = towerCardEntity;
            TowerSettings = towerSettings;
            _service = service;
            _container = container;
            // _uiManager = container.Resolve<MainMenuUIManager>();
        }


        public Vector3 GetPosition()
        {
            return new Vector3(Position.x, Position.y, 0);
        }

        public void RequestOpenPopupTowerCard()
        {
            _container.Resolve<Subject<TowerCardViewModel>>().OnNext(this);
         //   _uiManager.OpenPopupTowerCard(this);
        }
    }
}