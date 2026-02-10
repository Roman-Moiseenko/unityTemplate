using System;
using System.Collections.Generic;
using System.Linq;
using DI;
using Game.GamePlay.Commands.TowerCommand;
using Game.GamePlay.Commands.WarriorCommands;
using Game.GamePlay.Fsm;
using Game.GamePlay.Root;
using Game.GamePlay.View.Towers;
using Game.GameRoot.Commands;
using Game.Settings.Gameplay.Entities.Tower;
using Game.State.Inventory;
using Game.State.Inventory.TowerCards;
using Game.State.Maps.Mobs;
using Game.State.Maps.Shots;
using Game.State.Maps.Towers;
using Game.State.Research;
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
        private readonly FsmTower _fsmTower;
        private readonly FsmWave _fsmWave;
        private readonly ObservableList<TowerViewModel> _allTowers = new();
        private readonly Dictionary<int, TowerViewModel> _towersMap = new();
        //Кешируем параметры башен на карте
        private readonly Dictionary<string, List<TowerLevelSettings>> _towerSettingsMap = new();
        private readonly GameplayStateProxy _gameplayState;
        private readonly DIContainer _container;
        private readonly GameplayBoosters _gameplayBoosters;

        public Dictionary<string, Dictionary<TowerParameterType, float>> TowerBoosters = new();

        /**
         * При загрузке создаем все view-модели из реактивного списка всех строений.
         * Подписываемся на событие добавления в массив Proxy сущностей
        */
        public TowersService(
            GameplayStateProxy gameplayState,
            TowersSettings towersSettings,
            GameplayEnterParams gameplayEnterParams,
            DIContainer container
        )
        {
            _container = container;
            _gameplayState = gameplayState;
            
            _fsmTower = container.Resolve<FsmTower>();
            _fsmWave = container.Resolve<FsmWave>();
            _placementService = container.Resolve<PlacementService>();
            _cmd = container.Resolve<ICommandProcessor>();
            
            var towerEntities = gameplayState.Towers;
            _baseTowerCards = gameplayEnterParams.Towers; //Базовые настройки колоды
            _gameplayBoosters = gameplayEnterParams.GameplayBoosters; //TODO Передать в башни _castleResearch.TowerDamage 
            
            
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

            foreach (var towerCardData in _baseTowerCards)
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
            //Кешируем бустеры для башен по типам Defence
            CalculateBoosters();
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
            TowerViewModel towerViewModel;

            if (towerEntity.IsPlacement)
            {
                towerViewModel = new TowerPlacementViewModel(
                    towerEntity, 
                    _container,
                    _fsmWave, 
                    _placementService); //3
            }
            else
            {
                towerViewModel = new TowerAttackViewModel(towerEntity, _container); //3
            }
            //TODO Баф и Дебафф башни
            
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
        
        public void SetPlacement(int uniqueId, Vector2Int position)
        {
            foreach (var towerEntity in _gameplayState.Towers)
            {
                if (towerEntity.UniqueId == uniqueId)
                {
                    towerEntity.Placement.OnNext(position);
                    var command = new CommandSaveGameState();
                    _cmd.Process(command);
                    return;
                }
            }
        }


        public ShotData ShotCalculation(TowerEntity towerEntity, MobDefence mobDefence)
        {
            var damageBooster = 0f;
            var criticalBooster = 0f;
            var boosters = TowerBoosters[towerEntity.ConfigId];

            if (boosters.TryGetValue(TowerParameterType.Damage, out var damage)) damageBooster = damage;
            if (boosters.TryGetValue(TowerParameterType.Critical, out var critical)) criticalBooster = critical;
            
            return towerEntity.ShotCalculation(mobDefence, damageBooster, criticalBooster);
        }


        private void CalculateBoosters()
        {

            //бустеры общие
            var damageBooster = _gameplayBoosters.TowerDamage;
            var criticalBooster = _gameplayBoosters.TowerCritical;
            var speedBooster = _gameplayBoosters.TowerSpeed;
            var distanceBooster = _gameplayBoosters.TowerDistance;
            //бустеры общие от героя
            if (_gameplayBoosters.HeroTowerBust.TryGetValue(TowerParameterType.Damage, out var damage))
                damageBooster += damage;
            if (_gameplayBoosters.HeroTowerBust.TryGetValue(TowerParameterType.Critical, out var critical))
                criticalBooster += critical;
            if (_gameplayBoosters.HeroTowerBust.TryGetValue(TowerParameterType.Speed, out var speed))
                speedBooster += speed;
            if (_gameplayBoosters.HeroTowerBust.TryGetValue(TowerParameterType.Distance, out var distance))
                distanceBooster += distance;
            
            //бустеры от типа защиты и от наличия параметра в карточке
            foreach (var towerCard in _baseTowerCards)
            {
                //Фильтруем по наличию параметра в карточке башни
                var isDamage = towerCard.Parameters.TryGetValue(TowerParameterType.Damage, out _) ||
                               towerCard.Parameters.TryGetValue(TowerParameterType.DamageArea, out _);
                var isCritical = towerCard.Parameters.TryGetValue(TowerParameterType.Critical, out _);
                var isSpeed = towerCard.Parameters.TryGetValue(TowerParameterType.Speed, out _);
                var isDistance = towerCard.Parameters.TryGetValue(TowerParameterType.Distance, out _) || 
                                 towerCard.Parameters.TryGetValue(TowerParameterType.MaxDistance, out _);
                
                var damageBoosterTower = damageBooster;
                var criticalBoosterTower = criticalBooster;
                var speedBoosterTower = speedBooster;
                var distanceBoosterTower = distanceBooster;
                
                //бустеры от типа Defence о героя
                if (_gameplayBoosters.HeroTowerDefenceBust.TryGetValue(towerCard.Defence, out var parameterDatas))
                {
                    if (parameterDatas.TryGetValue(TowerParameterType.Damage, out var damageDefence))
                        damageBoosterTower += damageDefence;
                    if (parameterDatas.TryGetValue(TowerParameterType.Critical, out var criticalDefence))
                        criticalBoosterTower += criticalDefence;   
                    if (parameterDatas.TryGetValue(TowerParameterType.Speed, out var speedDefence))
                        speedBoosterTower += speedDefence;
                    if (parameterDatas.TryGetValue(TowerParameterType.Distance, out var distanceDefence))
                        distanceBoosterTower += distanceDefence;
                }

                Dictionary<TowerParameterType, float> boosters = new(); 
                
                if (isDamage && damageBoosterTower != 0) boosters.Add(TowerParameterType.Damage, damageBoosterTower);    
                if (isCritical && criticalBoosterTower != 0) boosters.Add(TowerParameterType.Critical, criticalBoosterTower);
                if (isSpeed && speedBoosterTower != 0) boosters.Add(TowerParameterType.Speed, speedBoosterTower);
                if (isDistance && distanceBoosterTower != 0) boosters.Add(TowerParameterType.Distance, distanceBoosterTower);
                TowerBoosters.Add(towerCard.ConfigId, boosters);
            }
        }
    }
}