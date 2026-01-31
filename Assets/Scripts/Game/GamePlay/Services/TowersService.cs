using System;
using System.Collections.Generic;
using System.Linq;
using DI;
using Game.GamePlay.Commands.TowerCommand;
using Game.GamePlay.Commands.WarriorCommands;
using Game.GamePlay.Fsm;
using Game.GamePlay.View.Towers;
using Game.GameRoot.Commands;
using Game.Settings.Gameplay.Entities.Tower;
using Game.State.Inventory;
using Game.State.Inventory.TowerCards;
using Game.State.Maps.Towers;
using Game.State.Root;
using MVVM.CMD;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.Services
{
    public class TowersService
    {
        public readonly Dictionary<string,
            Dictionary<TowerParameterType, TowerParameterData>> TowerParametersMap = new();
        public IObservableCollection<TowerViewModel> AllTowers =>
            _allTowers; //Интерфейс менять нельзя, возвращаем через динамический массив
        
        public readonly ObservableDictionary<string, int> Levels = new(); //Уровни башен по типам ConfigId

        private readonly List<TowerCardData> _baseTowerCards; //
        private readonly ICommandProcessor _cmd;
        private readonly PlacementService _placementService;
        private readonly WarriorService _warriorService;
        private readonly FsmTower _fsmTower;
        private readonly ObservableList<TowerViewModel> _allTowers = new();
        private readonly Dictionary<int, TowerViewModel> _towersMap = new();
        //Кешируем параметры башен на карте
        private readonly Dictionary<string, List<TowerLevelSettings>> _towerSettingsMap = new();
        private readonly GameplayStateProxy _gameplayState;


        /**
         * При загрузке создаем все view-модели из реактивного списка всех строений.
         * Подписываемся на событие добавления в массив Proxy сущностей
        */
        public TowersService(
            GameplayStateProxy gameplayState,
            TowersSettings towersSettings,
            List<TowerCardData> baseTowerCards, //Базовые настройки колоды
            ICommandProcessor cmd,
            PlacementService placementService,
            WarriorService warriorService,
            FsmTower fsmTower
        )
        {
            _gameplayState = gameplayState;
            var towerEntities = gameplayState.Towers;
            _baseTowerCards = baseTowerCards;
            _cmd = cmd;
            _placementService = placementService;
            _warriorService = warriorService;
            _fsmTower = fsmTower;

            //Кешируем настройки зданий / объектов
            foreach (var towerSettings in towersSettings.AllTowers)
            {
                _towerSettingsMap[towerSettings.ConfigId] = towerSettings.GameplayLevels;
                Levels[towerSettings.ConfigId] = 1;
            }
            //Кешируем уровень башни по конфигу, если башня этого типа есть на карте
            foreach (var towerEntity in towerEntities)
            {
                Levels[towerEntity.ConfigId] = towerEntity.Level.CurrentValue;
            }

            foreach (var towerCardData in baseTowerCards)
            {
                var param = new Dictionary<TowerParameterType, TowerParameterData>(); //Базовые параметры из колоды
                //Делаем копию параметров
                foreach (var parameterData in towerCardData.Parameters)
                {
                    param.Add(parameterData.Key, parameterData.Value.GetCopy());
                }
                TowerParametersMap.Add(towerCardData.ConfigId, param);
                for (var i = 1; i <= Levels[towerCardData.ConfigId]; i++)
                {
                    UpdateParams(towerCardData.ConfigId, i); //Увеличиваем параметры по геймплей уровню башни
                }
            }

            foreach (var towerEntity in towerEntities)
            {
                towerEntity.Parameters =
                    TowerParametersMap[towerEntity.ConfigId]; //Присваиваем по ссылке параметры башни
                CreateTowerViewModel(towerEntity);
            }

            towerEntities.ObserveAdd().Subscribe(e =>
            {
                var towerEntity = e.Value;
                towerEntity.Level.Value = Levels[towerEntity.ConfigId]; //Устанавливаем уровень апгрейда
                towerEntity.Parameters = TowerParametersMap[towerEntity.ConfigId];
                CreateTowerViewModel(towerEntity); //Создаем View Model
            });
            //Если у сущности изменился уровень, меняем его и во вью-модели
            towerEntities.ObserveRemove().Subscribe(e => RemoveTowerViewModel(e.Value));

            Levels.ObserveChanged().Subscribe(x =>
            {
                var configId = x.NewItem.Key;
                var newLevel = x.NewItem.Value;
                UpdateParams(configId, newLevel);

                foreach (var towerEntity in towerEntities)
                {
                    if (towerEntity.ConfigId != configId) continue;
                    towerEntity.Level.OnNext(newLevel);
                }
            });
        }

        

        private void UpdateParams(string configId, int level)
        {
            var levelSettings = _towerSettingsMap[configId].FirstOrDefault(l => l.Level == level);
            if (levelSettings == null) throw new Exception("Не найдены параметры башни " + configId);

            if (!TowerParametersMap.TryGetValue(configId, out var parameters)) return;

            foreach (var settingsParameter in levelSettings.Parameters)
            {
                if (parameters.TryGetValue(settingsParameter.ParameterType, out var parameter))
                {
                    parameter.Value *= 1 + settingsParameter.Value / 100;
                }
            }
        }

        public bool PlaceTower(string towerTypeId, Vector2Int position)
        {
            var command = new CommandPlaceTower(towerTypeId, position);
            command.Placement = _placementService.GetDirectionTower(position) + position;
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
            var towerViewModel = new TowerViewModel(towerEntity, _gameplayState, this, _fsmTower); //3
            var directionTower = _placementService.GetDirectionTower(towerEntity.Position.CurrentValue);
            towerViewModel.SetDirection(directionTower);
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

        public ReactiveProperty<bool> LevelUpTower(string configId)
        {
            TowerViewModel model = null;
            //Находим первую модель текущего типа башни, для отслеживания завершения анимации
            foreach (var towerViewModel in _allTowers)
            {
                if (towerViewModel.ConfigId != configId) continue;
                model = towerViewModel;
                break;
            }
            //Если модель не найдена, выйти за паузы сразу.
            if (model == null) return new ReactiveProperty<bool>(true);
            
            model.FinishEffectLevelUp.Value = false; //Флаг - Анимация еще не завершена. 
            
            //Повышаем уровень башен
            var command = new CommandTowerLevelUp(configId);
            //Если команда не выполнилась, выйти за паузы сразу
            if (!_cmd.Process(command)) return new ReactiveProperty<bool>(true);
            Levels[configId] += 1;
            return model.FinishEffectLevelUp;
        }

        
        public void ReplaceTower(int cardUniqueId, object cardUniqueId2)
        {
            //TODO Меняем местами две башни
        }

        /**
         * Список доступных башен на текущем уровне
         */
        public Dictionary<string, TypeEpicCard> GetAvailableTowers()
        {
            var towers = new Dictionary<string, TypeEpicCard>();

            foreach (var towerCard in _baseTowerCards)
            {
                towers.Add(towerCard.ConfigId, towerCard.EpicLevel);
            }

            return towers;
        }

        /**
         * Список доступных башен для апгрейда, должна быть построена и иметь уровень < 6
         */
        public Dictionary<string, int> GetAvailableUpgradeTowers()
        {
            var towers = new Dictionary<string, int>();
            foreach (var towerViewModel in _allTowers) //Все построенные башни
            {
                if (Levels[towerViewModel.ConfigId] < 6)
                    towers.TryAdd(towerViewModel.ConfigId, Levels[towerViewModel.ConfigId]); //Добавлять один раз
            }
            return towers;
        }

        /**
         * Методы для башен Placement с Warriors
         */   
        
        public bool IsDeadAllWarriors(TowerEntity towerEntity)
        {
            if (!towerEntity.IsPlacement) return false;
            return _warriorService.IsDeadAllWarriors(towerEntity.UniqueId);
        }
        
        public void AddWarriorsTower(TowerEntity towerEntity)
        {
            if (!towerEntity.IsPlacement) return;
            _warriorService.AddWarriorsTower(towerEntity);
        }
        
        
        //public 


        //
        public void SetPlacement()
        {
            var command = new CommandSaveGameState();
            _cmd.Process(command);
        }

        public void ResumePlacement(int uniqueId, Vector2Int position)
        {
            foreach (var towerEntity in _gameplayState.Towers)
            {
                if (towerEntity.UniqueId == uniqueId)
                {
                    towerEntity.Placement.OnNext(position);
                    break;
                }
            }
        }
    }
}