using System;
using System.Collections.Generic;
using Game.GamePlay.View.Buildings;
using Game.GamePlay.View.Grounds;
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
        private readonly Dictionary<int, GroundBinder> _createGroundsMap = new();

        private readonly CompositeDisposable _disposables = new();

        public void Bind(WorldGameplayRootViewModel viewModel)
        {
            Debug.Log("viewModel: " + JsonConvert.SerializeObject(viewModel.AllBuildings, Formatting.Indented));
            Debug.Log("viewModel: " + JsonConvert.SerializeObject(viewModel.AllGrounds, Formatting.Indented));
            foreach (var buildingViewModel in viewModel.AllBuildings)
                CreateBuilding(buildingViewModel);
            _disposables.Add(
                viewModel.AllBuildings.ObserveAdd().Subscribe(e => CreateBuilding(e.Value))
            );
            _disposables.Remove(
                viewModel.AllBuildings.ObserveRemove().Subscribe(e => DestroyBuilding(e.Value))
            );
            
            foreach (var groundViewModel in viewModel.AllGrounds)
                CreateGround(groundViewModel);
            /*
            _disposables.Add(
                viewModel.AllGrounds.ObserveAdd().Subscribe(e => CreateGround(e.Value))
            );
            _disposables.Remove(
                viewModel.AllGrounds.ObserveRemove().Subscribe(e => DestroyGround(e.Value))
            );
            */
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }

        private void CreateBuilding(BuildingViewModel buildingViewModel)
        {
            var buildingLevel = buildingViewModel.Level;
            var buildingType = buildingViewModel.ConfigId;
            
            var prefabBuildingLevelPath = $"Prefabs/Gameplay/Buildings/{buildingType}/Level_{buildingLevel}"; //Перенести в настройки уровня
            var buildingPrefab = Resources.Load<BuildingBinder>(prefabBuildingLevelPath);
            Debug.Log(prefabBuildingLevelPath);

            var createdBuilding = Instantiate(buildingPrefab, transform);
            createdBuilding.Bind(buildingViewModel);
            _createBuildingsMap[buildingViewModel.BuildingEntityId] = createdBuilding;
        }

        private void CreateGround(GroundViewModel groundViewModel)
        {
            var groundType = groundViewModel.ConfigId;
            var prefabGroundPath = $"Prefabs/Gameplay/Map/Grounds/{groundType}"; //Перенести в настройки уровня
            Debug.Log(prefabGroundPath);
            var groundPrefab = Resources.Load<GroundBinder>(prefabGroundPath);
//            Debug.Log(JsonConvert.SerializeObject(groundPrefab, Formatting.Indented));
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
        
        private void DestroyGround(GroundViewModel groundViewModel)
        {
            if (_createGroundsMap.TryGetValue(groundViewModel.GroundEntityId, out var groundBinder))
            {
                Destroy(groundBinder.gameObject);
                _createGroundsMap.Remove(groundViewModel.GroundEntityId);
            }
        }
    }
}