using System;
using System.Collections.Generic;
using Game.GamePlay.View.Buildins;
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

        private readonly CompositeDisposable _disposables = new();

        public void Bind(WorldGameplayRootViewModel viewModel)
        {
            foreach (var buildingViewModel in viewModel.AllBuildings)
            {
                CreateBuilding(buildingViewModel);
            }

            _disposables.Add(
                viewModel.AllBuildings.ObserveAdd().Subscribe(
                    e => CreateBuilding(e.Value)
                )
            );
            _disposables.Remove(
                viewModel.AllBuildings.ObserveRemove().Subscribe(
                    e => DestroyBuilding(e.Value)
                )
            );
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }

        private void CreateBuilding(BuildingViewModel buildingViewModel)
        {
            var buildingLevel = buildingViewModel.Level;
            var buildingType = buildingViewModel.TypeId;
           // var prefabName = buildingViewModel.GetLevelSettings(buildingLevel).prefab;
            
            var prefabBuildingLevelPath = $"Prefabs/Gameplay/Buildings/{buildingType}/Level_{buildingLevel}"; //Перенести в настройки уровня
            var buildingPrefab = Resources.Load<BuildingBinder>(prefabBuildingLevelPath);
            
            var createdBuilding = Instantiate(buildingPrefab, transform);
            createdBuilding.Bind(buildingViewModel);
            _createBuildingsMap[buildingViewModel.BuildingEntityId] = createdBuilding;
        }

        private void DestroyBuilding(BuildingViewModel buildingViewModel)
        {
            if (_createBuildingsMap.TryGetValue(buildingViewModel.BuildingEntityId, out var buildingBinder))
            {
                Destroy(buildingBinder.gameObject);
                _createBuildingsMap.Remove(buildingViewModel.BuildingEntityId);
            }
        }
    }
}