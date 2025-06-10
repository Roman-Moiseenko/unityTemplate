
using System.Collections.Generic;
using Game.GamePlay.Commands.TowerCommand;
using Game.GamePlay.View.Towers;
using Game.Settings.Gameplay.Entities.Tower;
using Game.State.Entities;
using Game.State.Maps.Towers;
using MVVM.CMD;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.Services
{
    public class TowersService
    {
        private readonly ICommandProcessor _cmd;

        private readonly ObservableList<TowerViewModel> _allTowers = new();
        private readonly Dictionary<int, TowerViewModel> _towersMap = new();
        private readonly Dictionary<string, TowerSettings> _towerSettingsMap = new();
        public IObservableCollection<TowerViewModel> AllTowers => _allTowers; //Интерфейс менять нельзя, возвращаем через динамический массив
        
        /**
         * При загрузке создаем все view-модели из реактивного списка всех строений 
         * Подписываемся на событие добавления в массив Proxy сущностей
        */
         
        public TowersService(
            IObservableCollection<Entity> entities,
            TowersSettings towersSettings, 
            ICommandProcessor cmd
            )
        {
            _cmd = cmd;

            //Кешируем настройки зданий / обектов
            foreach (var towerSettings in towersSettings.AllTowers)
            {
                _towerSettingsMap[towerSettings.ConfigId] = towerSettings;
            }
            
            foreach (var entity in entities)
            {
                if (entity is TowerEntity towerEntity) CreateTowerViewModel(towerEntity);
            }
            
            //Подписка на добавление новых view-моделей текущего класса
            entities.ObserveAdd().Subscribe(e =>
            {
                if (e.Value is TowerEntity towerEntity) CreateTowerViewModel(towerEntity);
            });
            // и на удаление
            entities.ObserveRemove().Subscribe(e =>
            {
                if (e.Value is TowerEntity towerEntity) RemoveTowerViewModel(towerEntity);
            });

        }


        public bool PlaceTower(string towerTypeId, Vector2Int position)
        {
            var command = new CommandPlaceTower(towerTypeId, position);
            return _cmd.Process(command);
        }

        public bool MoveTower(int towerId, Vector2Int position)
        {
            return false;
        }

        public bool DeleteTower(int towerId)
        {
            return false;
        }
        
        /**
         * 1. По параметрам создается сущность Tower
         * 2. Оборачивается Proxy для навешивания реактивности и событий
         * 3. На основе Proxy сущности создается view-модель
         * 4. Модель добавляем в словарь всех моделей данного класса
         * 5. Кешируем Id и view-модели
         */


        private void CreateTowerViewModel(TowerEntity towerEntity)
        {
            var towerViewModel = new TowerViewModel(towerEntity, _towerSettingsMap[towerEntity.ConfigId], this); //3
            _allTowers.Add(towerViewModel); //4
            _towersMap[towerEntity.UniqueId] = towerViewModel;
        }

        /**
         * Удаляем объект из списка моделей и из кеша
         */

        private void RemoveTowerViewModel(TowerEntity towerEntity)
        {
            if (_towersMap.TryGetValue(towerEntity.UniqueId, out var towerViewModel))
            {
                _allTowers.Remove(towerViewModel);
                _towersMap.Remove(towerEntity.UniqueId);
            }
        }
    }
}
