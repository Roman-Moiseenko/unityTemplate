using System;
using System.Collections.Generic;
using Game.GamePlay.View.Buildings;
using Game.GamePlay.View.Grounds;
using Game.GamePlay.View.Towers;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Game.GamePlay.Root.View
{
    public class WorldGameplayRootBinder : MonoBehaviour
    {
        //      [SerializeField] private BuildingBinder _prefabBuilding;
        private readonly Dictionary<int, BuildingBinder> _createBuildingsMap = new();
        private readonly Dictionary<int, TowerBinder> _createTowersMap = new();
        private readonly Dictionary<int, GroundBinder> _createGroundsMap = new();
        private readonly CompositeDisposable _disposables = new();


        private WorldGameplayRootViewModel _viewModel;

        public void Bind(WorldGameplayRootViewModel viewModel)
        {
            _viewModel = viewModel;
            //1. Создаем все объекты мира из Прехабов
            //2. Подписываемся на добавление объектов в список (Создать) и на удаление (Уничтожить)
            foreach (var towerViewModel in viewModel.AllTowers)
                CreateTower(towerViewModel);
            
            _disposables.Add(
                viewModel.AllTowers.ObserveAdd().Subscribe(e => CreateTower(e.Value))
            );
            _disposables.Remove(
                viewModel.AllTowers.ObserveRemove().Subscribe(e => DestroyTower(e.Value))
            );

            foreach (var buildingViewModel in viewModel.AllBuildings)
                CreateBuilding(buildingViewModel);
            _disposables.Add(
                viewModel.AllBuildings.ObserveAdd().Subscribe(e => CreateBuilding(e.Value))
            );            
            
            
            foreach (var groundViewModel in viewModel.AllGrounds)
                CreateGround(groundViewModel);
            _disposables.Add(
                viewModel.AllGrounds.ObserveAdd().Subscribe(e => CreateGround(e.Value))
            );
            _disposables.Remove(
                viewModel.AllGrounds.ObserveRemove().Subscribe(e => DestroyGround(e.Value))
            );
            
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }

        private void CreateBuilding(BuildingViewModel buildingViewModel)
        {
            var buildingLevel = buildingViewModel.Level;
            var buildingType = buildingViewModel.ConfigId;

            var prefabBuildingLevelPath =
                $"Prefabs/Gameplay/Buildings/{buildingType}/Level_{buildingLevel}"; //Перенести в настройки уровня
            var buildingPrefab = Resources.Load<BuildingBinder>(prefabBuildingLevelPath);
            Debug.Log(prefabBuildingLevelPath);

            var createdBuilding = Instantiate(buildingPrefab, transform);
            createdBuilding.Bind(buildingViewModel);
            _createBuildingsMap[buildingViewModel.BuildingEntityId] = createdBuilding;
        }

        private void CreateTower(TowerViewModel towerViewModel)
        {
            var towerLevel = towerViewModel.EpicLevel;
            var towerType = towerViewModel.ConfigId;

            var prefabTowerLevelPath =
                $"Prefabs/Gameplay/Towers/{towerType}/Level_{towerLevel}"; //Перенести в настройки уровня
            var towerPrefab = Resources.Load<TowerBinder>(prefabTowerLevelPath);

            var createdTower = Instantiate(towerPrefab, transform);
            createdTower.Bind(towerViewModel);
            _createTowersMap[towerViewModel.TowerEntityId] = createdTower;
        }

        private void CreateGround(GroundViewModel groundViewModel)
        {
            var groundType = groundViewModel.ConfigId;
            var prefabGroundPath = $"Prefabs/Gameplay/Map/Grounds/{groundType}"; //Перенести в настройки уровня
            var groundPrefab = Resources.Load<GroundBinder>(prefabGroundPath);
            var createdGround = Instantiate(groundPrefab, transform);
            createdGround.Bind(groundViewModel);
            _createGroundsMap[groundViewModel.GroundEntityId] = createdGround;
        }
        
        private void DestroyBuilding(BuildingViewModel buildingViewModel)
        {
            if (_createBuildingsMap.TryGetValue(buildingViewModel.BuildingEntityId, out var buildingBinder))
            {
                Destroy(buildingBinder.gameObject);
                _createBuildingsMap.Remove(buildingViewModel.BuildingEntityId);
            }
        }

        private void DestroyTower(TowerViewModel towerViewModel)
        {
            if (_createTowersMap.TryGetValue(towerViewModel.TowerEntityId, out var towerBinder))
            {
                Destroy(towerBinder.gameObject);
                _createTowersMap.Remove(towerViewModel.TowerEntityId);
            }
        }

        private void DestroyGround(GroundViewModel groundViewModel)
        {
            if (_createGroundsMap.TryGetValue(groundViewModel.GroundEntityId, out var groundBinder))
            {
                Destroy(groundBinder.gameObject);
                _createGroundsMap.Remove(groundViewModel.GroundEntityId);
            }
        }

        private void Update()
        {
            //Проверка мышки и состояния
            if (Input.GetMouseButtonDown(0))
            {
                var cursorPosition = Input.mousePosition;
             //   Ray ray = Camera.main.ScreenPointToRay(cursorPosition);
             if (Physics.Raycast(Camera.main.ScreenPointToRay(cursorPosition), out RaycastHit hit))
             {
                 print(hit.transform.name);
                 print(hit.transform.position);
             }
             
           //     Debug.Log(Input.mousePosition.x + ", "+ Input.mousePosition.y + ", " + Input.mousePosition.z);
                
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // _viewModel.HandleTestInput();
            }
        }

    }
}