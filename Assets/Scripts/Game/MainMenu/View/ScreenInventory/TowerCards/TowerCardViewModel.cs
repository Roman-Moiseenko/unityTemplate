using Game.MainMenu.Services;
using Game.Settings.Gameplay.Entities.Tower;
using Game.State.Inventory.TowerCards;
using UnityEngine;

namespace Game.MainMenu.View.ScreenInventory.TowerCards
{
    public class TowerCardViewModel
    {
        
        private readonly TowerCard _towerCardEntity;
        public TowerCard TowerCard => _towerCardEntity;
        private readonly TowerSettings _towerSettings;
        public Vector2Int Position = Vector2Int.zero;

        private readonly TowerCardService _service;
        //  private TowerCard _towerCardEntity;

        public int IdTowerCard => _towerCardEntity.UniqueId;


        public TowerCardViewModel(
            TowerCard towerCardEntity, 
            TowerSettings towerSettings, 
            TowerCardService service
            )
        {
            _towerCardEntity = towerCardEntity;
            _towerSettings = towerSettings;
            _service = service;
        }


        public Vector3 GetPosition()
        {
            return new Vector3(Position.x, Position.y, 0);
        }
    }
}