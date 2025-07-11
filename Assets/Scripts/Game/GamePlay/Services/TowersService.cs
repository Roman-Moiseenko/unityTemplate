﻿using System;
using System.Collections.Generic;
using System.Linq;
using Game.GamePlay.Commands.TowerCommand;
using Game.GamePlay.View.Towers;
using Game.Settings.Gameplay.Entities.Tower;
using Game.State.Entities;
using Game.State.Inventory.TowerCards;
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
        private readonly IObservableCollection<TowerEntity> _towerEntities; //кешируем
        private readonly List<TowerCardData> _baseTowerCards; //
        private readonly ICommandProcessor _cmd;
        private readonly PlacementService _placementService;

        private readonly ObservableList<TowerViewModel> _allTowers = new();
        private readonly Dictionary<int, TowerViewModel> _towersMap = new();
        private readonly Dictionary<string, List<TowerLevelSettings>> _towerSettingsMap = new();

        public IObservableCollection<TowerViewModel> AllTowers =>
            _allTowers; //Интерфейс менять нельзя, возвращаем через динамический массив

        public readonly ObservableDictionary<string, int> Levels = new(); //Уровни башен по типам ConfigId


        /**
         * При загрузке создаем все view-модели из реактивного списка всех строений.
         * Подписываемся на событие добавления в массив Proxy сущностей
        */
        public TowersService(
            IObservableCollection<TowerEntity> towerEntities,
            TowersSettings towersSettings,
            List<TowerCardData> baseTowerCards, //Базовые настройки колоды
            ICommandProcessor cmd,
            PlacementService placementService
        )
        {
            //Debug.Log("Входные данные для башен " + JsonConvert.SerializeObject(baseTowerCards, Formatting.Indented));
            _towerEntities = towerEntities;
            _baseTowerCards = baseTowerCards;
            _cmd = cmd;
            _placementService = placementService;

            //Кешируем настройки зданий / объектов
            
            foreach (var towerSettings in towersSettings.AllTowers)
            {
                _towerSettingsMap[towerSettings.ConfigId] = towerSettings.GameplayLevels;
                Levels[towerSettings.ConfigId] = 1;
            }

            foreach (var towerEntity in towerEntities)
            {
                Levels[towerEntity.ConfigId] = towerEntity.Level.CurrentValue;
                CreateTowerViewModel(towerEntity);
            }

            //Подписка на добавление новых view-моделей текущего класса
            towerEntities.ObserveAdd().Subscribe(e =>
            {
                    e.Value.Level.Value = Levels[e.Value.ConfigId]; //Устанавливаем уровень апгрейда
                    CreateTowerViewModel(e.Value); //Создаем View Model
            });
            //Если у сущности изменился уровень, меняем его и во вью-модели
            towerEntities.ObserveRemove().Subscribe(e => RemoveTowerViewModel(e.Value));

            Levels.ObserveChanged().Subscribe(x =>
            {
                var configId = x.NewItem.Key;
                var newLevel = x.NewItem.Value;
                var towerCardBaseSetting = baseTowerCards.FirstOrDefault(card => card.ConfigId == configId);
                var levelSettings = _towerSettingsMap[configId].FirstOrDefault(l => l.Level == newLevel);
                //Debug.Log(JsonConvert.SerializeObject(_towerEntities, Formatting.Indented));
                //Debug.Log(_towerEntities.Count);
                foreach (var towerEntity in _towerEntities)
                {
                    if (towerEntity.ConfigId != configId) continue;
                    foreach (var settingsParameter in levelSettings.Parameters)
                    {
                        if (towerEntity.Parameters.TryGetValue(settingsParameter.ParameterType, out var parameter))
                        {
                            parameter.Value.Value *= 1 + settingsParameter.Value / 100;
                        }
                        //находим в towerCardBaseSetting settingsParameter и меняем параметр в %%
                    }
                    RemoveTowerViewModel(towerEntity); //Удаляем все модели viewModel.ConfigId == x.NewItem.Key
                    CreateTowerViewModel(towerEntity); //Создаем модели Заново
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
            try
            {
                var towerCardBaseSetting = _baseTowerCards.FirstOrDefault(card => card.ConfigId == towerEntity.ConfigId);
                if (towerCardBaseSetting == null) throw new Exception("Не найден параметр в настройках");
                foreach (var keyValue in towerCardBaseSetting.Parameters)
                {
                    towerEntity.Parameters.TryAdd(keyValue.Key, new TowerParameter(keyValue.Value));
                }
                var towerViewModel = new TowerViewModel(towerEntity, _towerSettingsMap[towerEntity.ConfigId], this); //3
                _allTowers.Add(towerViewModel); //4
                _towersMap[towerEntity.UniqueId] = towerViewModel;
            }
            catch (Exception e)
            {
               Debug.Log(e.Message);
                throw;
            }

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

        /**
         * Список доступных башен на текущем уровне
         */
        public Dictionary<string, int> GetAvailableTowers()
        {
            var towers = new Dictionary<string, int>();

            foreach (var towerCard in _baseTowerCards)
            {
                towers.Add(towerCard.ConfigId, Levels[towerCard.ConfigId]);
            }

            return towers;
            //return _baseTowerCards.Select(towerCard => towerCard.ConfigId).ToList();
        }

        /**
         * Список доступных башен для апгрейда, должна быть построена и иметь уровень <6
         */
        public Dictionary<string, int> GetAvailableUpgradeTowers()
        {
            var towers = new Dictionary<string, int>();

            foreach (var towerViewModel in _allTowers) //Все построенные башни
            {
                if (Levels[towerViewModel.ConfigId] < 6)
                {
                    towers.TryAdd(towerViewModel.ConfigId, Levels[towerViewModel.ConfigId]); //Добавлять один раз
                }
            }
            return towers;
            
        }
        
    }
}