
using System.Collections.Generic;
using Game.GamePlay.Commands;
using Game.GamePlay.View.Buildings;
using Game.Settings.Gameplay.Buildings;
using Game.State.CMD;
using Game.State.Entities;
using Game.State.Mergeable.Buildings;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.Services
{
    public class BuildingsService
    {
        private readonly ICommandProcessor _cmd;

        private readonly ObservableList<BuildingViewModel> _allBuildings = new();
        private readonly Dictionary<int, BuildingViewModel> _buildingsMap = new();
        private readonly Dictionary<string, BuildingSettings> _buildingSettingsMap = new();
        public IObservableCollection<BuildingViewModel> AllBuildings => _allBuildings; //Интерфейс менять нельзя, возвращаем через динамический массив
        
        /**
         * При загрузке создаем все view-модели из реактивного списка всех строений 
         * Подписываемся на событие добавления в массив Proxy сущностей
        */
         
        public BuildingsService(
            IObservableCollection<Entity> entities,
            BuildingsSettings buildingsSettings, 
            ICommandProcessor cmd
            )
        {
            
            _cmd = cmd;

            //Кешируем настройки зданий / обектов
            foreach (var buildingSettings in buildingsSettings.AllBuildings)
            {
                _buildingSettingsMap[buildingSettings.TypeId] = buildingSettings;
            }
            
            foreach (var entity in entities)
            {
                if (entity is BuildingEntity buildingEntity) CreateBuildingViewModel(buildingEntity);
            }
            
            //Подписка на добавление новых view-моделей текущего класса
            entities.ObserveAdd().Subscribe(e =>
            {
                if (e.Value is BuildingEntity buildingEntity) CreateBuildingViewModel(buildingEntity);
            });
            // и на удаление
            entities.ObserveRemove().Subscribe(e =>
            {
                if (e.Value is BuildingEntity buildingEntity) RemoveBuildingViewModel(buildingEntity);
            });

        }


        public bool PlaceBuilding(string buildingTypeId, Vector3Int position)
        {
            var command = new CommandPlaceBuilding(buildingTypeId, position);
            return _cmd.Process(command);
        }

        public bool MoveBuilding(int buildingId, Vector3Int position)
        {
            return false;
        }

        public bool boolDeleteBuilding(int buildingId)
        {
            return false;
        }


        /**
         * 1. По параметрам создается сущность Building
         * 2. Оборачивается Proxy для навешивания реактивности и событий
         * 3. На основе Proxy сущности создается view-модель
         * 4. Модель добавляем в словарь всех моделей данного класса
         * 5. Кешируем Id и view-модели
         */


        private void CreateBuildingViewModel(BuildingEntity buildingEntity)
        {
            var buildingViewModel = new BuildingViewModel(buildingEntity, _buildingSettingsMap[buildingEntity.ConfigId], this); //3
            _allBuildings.Add(buildingViewModel); //4
            _buildingsMap[buildingEntity.UniqueId] = buildingViewModel;
        }

        /**
         * Удаляем объект из списка моделей и из кеша
         */

        private void RemoveBuildingViewModel(BuildingEntity buildingEntity)
        {
            if (_buildingsMap.TryGetValue(buildingEntity.UniqueId, out var buildingViewModel))
            {
                _allBuildings.Remove(buildingViewModel);
                _buildingsMap.Remove(buildingEntity.UniqueId);
            }
        }
    }
}
