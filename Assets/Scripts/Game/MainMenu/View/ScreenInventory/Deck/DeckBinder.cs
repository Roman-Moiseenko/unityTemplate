using System;
using System.Collections.Generic;
using Game.MainMenu.View.ScreenInventory.SkillCards;
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
        private readonly Dictionary<int, SkillCardHexBinder> _createdSkillCardMap = new();
        private readonly List<Transform> _createdTowerCellMap = new();
        private readonly List<Transform> _createdSkillCellMap = new();
        private IDisposable _disposable;

        public void Bind(ScreenInventoryViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();

            StartEmptyCells();

            //Башни
            foreach (var towerCardViewModel in viewModel.TowerCardsDeck)
            {
                CreateTowerCard(towerCardViewModel);
                DestroyTowerCell();
            }
            
            viewModel.TowerCardsDeck.ObserveAdd().Subscribe(e =>
            {
                CreateTowerCard(e.Value);
                DestroyTowerCell();
            }).AddTo(ref d);
            viewModel.TowerCardsDeck.ObserveRemove().Subscribe(e =>
            {
                DestroyTowerCard(e.Value);
                CreateTowerCell();
            }).AddTo(ref d);
            
            //Навыки
            foreach (var skillCardViewModel in viewModel.SkillCardsDeck)
            {
                CreateSkillCard(skillCardViewModel);
                DestroySkillCell();
            }
            
            viewModel.SkillCardsDeck.ObserveAdd().Subscribe(e =>
            {
                CreateSkillCard(e.Value);
                DestroySkillCell();
            }).AddTo(ref d);
            viewModel.SkillCardsDeck.ObserveRemove().Subscribe(e =>
            {
                DestroySkillCard(e.Value);
                CreateSkillCell();
            }).AddTo(ref d);
            _disposable = d.Build();
        }

        private void StartEmptyCells()
        {
            for (var i = 0; i < 6; i++)
            {
                CreateTowerCell();
            }
            
            for (var i = 0; i < 2; i++)
            {
                CreateSkillCell();
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

        private void CreateSkillCard(SkillCardViewModel viewModel)
        {
            var prefabSkillCardPath =
                $"Prefabs/UI/MainMenu/ScreenInventory/SkillCardHex";
            var skillPrefab = Resources.Load<SkillCardHexBinder>(prefabSkillCardPath);
            var createdSkill = Instantiate(skillPrefab, containerSkillCards);
            createdSkill.Bind(viewModel);
            createdSkill.transform.SetSiblingIndex(0);
            _createdSkillCardMap[viewModel.IdSkillCard] = createdSkill;
        }
        private void CreateSkillCell()
        {
            var prefabSkillCellPath = $"Prefabs/UI/MainMenu/ScreenInventory/EmptySkillCard";
            var cellPrefab = Resources.Load<Transform>(prefabSkillCellPath);
            var createdCell = Instantiate(cellPrefab, containerSkillCards);
            _createdSkillCellMap.Add(createdCell);
        }
        private void DestroySkillCard(SkillCardViewModel viewModel)
        {
            if (_createdSkillCardMap.TryGetValue(viewModel.IdSkillCard, out var skillCardBinder))
            {
                Destroy(skillCardBinder.gameObject);
                _createdSkillCardMap.Remove(viewModel.IdSkillCard);
            }
        }
        private void DestroySkillCell()
        {
            var cell = _createdSkillCellMap[0];
            Destroy(cell.gameObject);
            _createdSkillCellMap.Remove(cell);
        }
        
        private void OnDestroy()
        {
            _disposable.Dispose();
        }
    }
}