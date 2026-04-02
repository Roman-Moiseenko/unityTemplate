using System;
using System.Collections.Generic;
using Game.MainMenu.View.ScreenInventory.TowerCards;
using ObservableCollections;
using R3;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.MainMenu.View.ScreenInventory.Deck
{
    public class DeckBinder : MonoBehaviour
    {
        [SerializeField] private List<Transform> towerCards = new(6);
        [SerializeField] private List<Transform> skillCards = new(2);
        [SerializeField] private Transform heroCard;
        [SerializeField] private Transform containerTowerCards;
        [SerializeField] private Transform containerSkillCards;

        private readonly Dictionary<int, TowerCardBinder> _createdTowerCardMap = new();
        private readonly List<Transform> _createdTowerCellMap = new();
        private IDisposable _disposable;

        public void Bind(ScreenInventoryViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();

            StartEmptyCells();

            
            foreach (var towerCardViewModel in viewModel.TowerCardsDeck)
            {
                CreateTowerCard(towerCardViewModel);
                DestroyTowerCell();
            }
            
            viewModel.TowerCardsDeck.ObserveAdd().Subscribe(e =>
            {
                // Устанавливаем индекс дочернего объекта childObject.SetSiblingIndex(1);
                CreateTowerCard(e.Value);
                DestroyTowerCell();
            }).AddTo(ref d);
            viewModel.TowerCardsDeck.ObserveRemove().Subscribe(e =>
            {
                DestroyTowerCard(e.Value);
                CreateTowerCell();
            }).AddTo(ref d);
            
            _disposable = d.Build();
        }

        private void StartEmptyCells()
        {
            for (var i = 0; i < 6; i++)
            {
                CreateTowerCell();
            }
        }
        
        private void CreateTowerCard(TowerCardViewModel viewModel)
        {
            var prefabTowerCardPath =
                $"Prefabs/UI/MainMenu/ScreenInventory/TowerCard";
            var towerPrefab = Resources.Load<TowerCardBinder>(prefabTowerCardPath);
            var createdTower = Instantiate(towerPrefab, containerTowerCards);
            createdTower.Bind(viewModel);
            createdTower.transform.SetSiblingIndex(0);
            _createdTowerCardMap[viewModel.IdTowerCard] = createdTower;
        }

        private void CreateTowerCell()
        {
            var prefabTowerCellPath = $"Prefabs/UI/MainMenu/ScreenInventory/EmptyTowerCard";
            var cellPrefab = Resources.Load<Transform>(prefabTowerCellPath);
            var createdCell = Instantiate(cellPrefab, containerTowerCards);
            //createdTower.Bind(viewModel);
            _createdTowerCellMap.Add(createdCell);
        }
        private void DestroyTowerCard(TowerCardViewModel viewModel)
        {
            if (_createdTowerCardMap.TryGetValue(viewModel.IdTowerCard, out var towerCardBinder))
            {
                Destroy(towerCardBinder.gameObject);
                _createdTowerCardMap.Remove(viewModel.IdTowerCard);
            }
        }
        private void DestroyTowerCell()
        {
            var cell = _createdTowerCellMap[0];
            Destroy(cell.gameObject);
            _createdTowerCellMap.Remove(cell);
        }

        private void OnDestroy()
        {
            _disposable.Dispose();
        }
    }
}