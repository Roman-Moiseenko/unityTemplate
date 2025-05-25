using System;
using System.Linq;
using Game.State.Entities.Buildings;
using Game.State.Root;
using ObservableCollections;
using R3;
using Scripts.Game.GameRoot.Services;
using UnityEngine;

namespace Game.GamePlay.Services
{
    /**
     * Внутренний сервис геймплея, зависящий от сервисов проекта
     */
    public class SomeGameplayService: IDisposable
    {
        private readonly SomeCommonService _someCommonService;
        private readonly GameStateProxy _gameState;
        
        public SomeGameplayService(GameStateProxy gameState, SomeCommonService someCommonService)
        {
            _someCommonService = someCommonService;
            _gameState = gameState;
            Debug.Log(GetType().Name + " был создан");
         /*   
            gameState.Buildings.ForEach(
                b => Debug.Log($"Building: {b.TypeId}"));
            
            
            gameState.Buildings.ObserveAdd().Subscribe(e => Debug.Log($"Building added: {e.Value.TypeId}"));
            gameState.Buildings.ObserveRemove().Subscribe(e => Debug.Log($"Building removeded: {e.Value.TypeId}"));
            
            
            AddBuilding("Здание 1");
            AddBuilding("Здание 2");
            AddBuilding("Здание 3");
            RemoveBuilding("Здание 2");
            
            */
        }

        public void Dispose()
        {
            Debug.Log("Чистка подписок");
        }

        private void AddBuilding(string buildingTypeId)
        {
            var building = new BuildingEntity
            {
                TypeId = buildingTypeId,
            };
            var buildingProxy = new BuildingEntityProxy(building);
            _gameState.Buildings.Add(buildingProxy);
        }

        private void RemoveBuilding(string buildingTypeId)
        {
            var buildingEntity = _gameState.Buildings.FirstOrDefault(b => b.TypeId == buildingTypeId);
            if (buildingEntity != null)
            {
                _gameState.Buildings.Remove(buildingEntity);
            }
        }
    }
}