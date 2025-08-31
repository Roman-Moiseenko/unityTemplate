using DI;
using Game.State.Inventory;
using Game.State.Inventory.TowerCards;
using R3;
using UnityEngine;

namespace Game.MainMenu.View.ScreenInventory.PopupBlacksmith.PrefabBinders
{
    public class TowerCardResourceViewModel
    {
        public TowerCard TowerCard => _towerCardEntity;
        public Vector3 Position = Vector3.zero;
        public int TowerEntityId => _towerCardEntity.UniqueId;
        public ReactiveProperty<bool> IsSelected = new();
        
        private readonly DIContainer _container;
        private readonly ReactiveProperty<bool> _limitedSelected;
        private readonly TowerCard _towerCardEntity;
        public string ConfigId => _towerCardEntity.ConfigId;
        public int Level => _towerCardEntity.Level.Value;
        public TypeEpicCard EpicLevel => _towerCardEntity.EpicLevel.Value;
        public string NameCard => _towerCardEntity.Name;


        public TowerCardResourceViewModel(
            TowerCard towerCardEntity, 
            DIContainer container,
            ReactiveProperty<bool> limitedSelected)
        {
            _towerCardEntity = towerCardEntity;
            _container = container;
            _limitedSelected = limitedSelected;
        }

        public void RequestSelectedTowerCard()
        {
            if (_limitedSelected.CurrentValue && !IsSelected.Value) return;
            IsSelected.Value = !IsSelected.Value;
        }
        
    }
}