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
        private readonly IObservableCollection<Entity> _entities; //кешируем
        private readonly ICommandProcessor _cmd;
        private readonly PlacementService _placementService;

        private readonly ObservableList<TowerViewModel> _allTowers = new();
        private readonly Dictionary<int, TowerViewModel> _towersMap = new();
        private readonly Dictionary<string, TowerSettings> _towerSettingsMap = new();

        public IObservableCollection<TowerViewModel> AllTowers =>
            _allTowers; //Интерфейс менять нельзя, возвращаем через динамический массив

        public readonly ObservableDictionary<string, int> Levels = new(); //Уровни башен по типам ConfigId


        /**
         * При загрузке создаем все view-модели из реактивного списка всех строений
         * Подписываемся на событие добавления в массив Proxy сущностей
        */
        public TowersService(
            IObservableCollection<Entity> entities,
            TowersSettings towersSettings,
            ICommandProcessor cmd,
            PlacementService placementService
        )
        {
            _entities = entities;
            _cmd = cmd;
            _placementService = placementService;

            //Кешируем настройки зданий / обектов
            foreach (var towerSettings in towersSettings.AllTowers)
            {
                _towerSettingsMap[towerSettings.ConfigId] = towerSettings;
                Levels[towerSettings.ConfigId] = 1;
            }

            foreach (var entity in entities)
            {
                if (entity is TowerEntity towerEntity) CreateTowerViewModel(towerEntity);
            }

            //Подписка на добавление новых view-моделей текущего класса
            entities.ObserveAdd().Subscribe(e =>
            {
                if (e.Value is TowerEntity towerEntity)
                {
                    towerEntity.Level.Value = Levels[towerEntity.ConfigId]; //Устанавливаем уровень апгрейда
                    CreateTowerViewModel(towerEntity); //Создаем View Model
   
                }
            });
            //Если у сущности изменился уровень, меняем его и во вью-модели
            entities.ObserveRemove().Subscribe(e =>
            {
                if (e.Value is TowerEntity towerEntity) RemoveTowerViewModel(towerEntity);
            });

            Levels.ObserveChanged().Subscribe(x =>
            {
                foreach (var entity in _entities)
                {
                    if (entity is TowerEntity towerEntity && towerEntity.ConfigId == x.NewItem.Key)
                    {
                        RemoveTowerViewModel(towerEntity); //TODO Удаляем все модели viewModel.ConfigId == x.NewItem.Key
                        CreateTowerViewModel(towerEntity); //TODO Создаем модели Заново
                    }
                }
            });
        }


        public bool PlaceTower(string towerTypeId, Vector2Int position)
        {
            var command = new CommandPlaceTower(towerTypeId, position);
            return _cmd.Process(command);
        }


        public bool MoveTower(int towerId, Vector2Int position)
        {
            var command = new CommandMoveTower(towerId, position);
            return _cmd.Process(command);
        }
        
        public bool DeleteTower(int towerId)
        {
            var command = new CommandDeleteTower(towerId);
            return _cmd.Process(command);
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

        public bool LevelUpTower(string configId)
        {
            //Повышаем уровень башен
            //Нужно для кеширования при строительстве новой башни и замены модели
            var command = new CommandTowerLevelUp(configId);
            if (_cmd.Process(command))
            {
                Levels[configId] += 1;
                return true;
            }

            return false;
        }

        public void ReplaceTower(int cardUniqueId, object cardUniqueId2)
        {
            //TODO Меняем местами две башни
        }
    }
}