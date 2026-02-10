using System;
using System.Collections.Generic;
using System.Linq;
using DI;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.GameplayStates;
using Game.GamePlay.Root;
using Game.Settings;
using Game.Settings.Gameplay.Entities.Tower;
using Game.State.Gameplay;
using Game.State.Inventory;
using Game.State.Maps.Rewards;
using Game.State.Research;
using Game.State.Root;
using ObservableCollections;
using R3;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.GamePlay.Services
{
    public class RewardProgressService
    {
        public ObservableList<RewardCurrencyEntity> RewardMaps = new();
        public ReactiveProperty<RewardEntity> RewardEntity = new();       
        
        private readonly GameplayStateProxy _gameplayState;
        private readonly DIContainer _container;
        private readonly GameSettings _gameSettings;
        private readonly TowersSettings _towersSettings;
        private readonly TowersService _towersService;
        private Dictionary<int, bool> _rewardsMap = new();
        private readonly GameplayBoosters _gameplayBoosters;

        public RewardProgressService(
            GameplayStateProxy gameplayState,
            DIContainer container,
            GameSettings gameSettings,
            GameplayEnterParams gameplayEnterParams
            //TODO Добавить SkillsSettings, HeroesSettings
        )
        {
            _gameplayState = gameplayState;
            _container = container;
            _gameSettings = gameSettings;
            _gameplayBoosters = gameplayEnterParams.GameplayBoosters;
            _towersService = container.Resolve<TowersService>();
            _towersSettings = gameSettings.TowersSettings;
            
            //gameplayState.Waves.
            //TODO Подписка на мобов из текущей волны, что на карте, при удалении => выдать награду
            
            var fsmGameplay = container.Resolve<FsmGameplay>();

            
            gameplayState.Mobs.ObserveRemove().Subscribe(e =>
            {
                //При удалении моба (когда IsDead => true) выдаем награду
                var mobEntity = e.Value;
                RewardKillMob(mobEntity.RewardCurrency, mobEntity.Position.CurrentValue);
            });
            
            gameplayState.Progress.Where(v => v >= 100).Subscribe(newValue =>
            {
                if (!fsmGameplay.IsStateGamePlay()) return;
                fsmGameplay.Fsm.SetState<FsmStateBuildBegin>();
            });
            
            //TODO Куда перенести
            fsmGameplay.Fsm.StateCurrent.Subscribe(newState =>
            {
                if (newState.GetType() == typeof(FsmStateBuild))
                {
                    //TODO Имитируем работу контроллера ввода
                    /*
                    var card = ((FsmStateBuild)newState).GetRewardCard();
                    card.Direction = 2;
                    card.Position = new Vector2Int(Random.Range(0, 5), Random.Range(0, 2));
                    fsm.Fsm.SetState<FsmStateBuildEnd>(card); */
                    //
                }

                if (newState.GetType() == typeof(FsmStateBuildEnd))
                {
                  //  Debug.Log("Построено");
                    if (gameplayState.Progress.Value >= 100) gameplayState.Progress.Value -= 100;
                //    Debug.Log("Прогресс остаточный = " + gameplayStateProxy.Progress.Value);

                gameplayState.ProgressLevel.Value++;
                    
                }
                
                if (newState.GetType() == typeof(FsmStateGamePlay))
                {
                    //При завершении строительства, если еще остались очки прогресса
                    if (gameplayState.Progress.Value >= 100)
                    {
                     //   Debug.Log("Прогресс остаточный > 100 ");
                       // var rewards = GenerateRewardProgress();
                        fsmGameplay.Fsm.SetState<FsmStateBuildBegin>();
                    }
                }
            });
            
            //Монетки долетели и удалились
            RewardMaps.ObserveRemove().Subscribe(r =>
            {
                gameplayState.ProgressUp();
                gameplayState.SoftCurrency.Value += r.Value.Currency;
            });
            RewardEntity.Where(r => r != null).Subscribe(reward =>
            {
                gameplayState.RewardEntities.Add(reward.Origin);
            });
        }

        public void RewardKillMob(int currency, Vector2 position)
        {
            RewardMaps.Add(new RewardCurrencyEntity
            {
                Position = position,
                Currency = (int)Mathf.Round(currency * (1 + _gameplayBoosters.RewardCurrency /100f)),
                //Доп.награда от исследования
            });
            //Алгоритм настройки получения награды
            //Частота получения награды из настроек - каждую 5 волну

            var waveReward = _gameSettings.MapsSettings.CommonSetting.rateRewardEntity;
            var waveCurrent = _gameplayState.CurrentWave.CurrentValue;
            if (waveCurrent % waveReward != 0) return;

            if (_rewardsMap.TryGetValue(waveCurrent, out var value)) return;
            
            
            var listConfig = new List<string>();
            foreach (var towerSettings in _towersSettings.AllTowers)
            {
                if (towerSettings.AvailableWave <= waveCurrent) listConfig.Add(towerSettings.ConfigId);
            }

            if (listConfig.Count == 0) throw new Exception("Исключительная ситуация");
            var reward = new RewardEntityData();
            
            var random = new System.Random();
            reward.RewardType = random.Next(0, 2) switch
            {
                0 => InventoryType.TowerCard,
                1 => InventoryType.TowerPlan,
                _ => reward.RewardType
            };

            reward.ConfigId = listConfig[random.Next(0, listConfig.Count)];
            reward.Amount = 1;
            //Добавляем награду карточку
            var rewardEntity = new RewardEntity(reward)
            {
                Position = position
            };
            //Debug.Log(JsonConvert.SerializeObject(reward, Formatting.Indented));
            RewardEntity.OnNext(rewardEntity);
            _rewardsMap.Add(waveCurrent, true);
            //_gameplayState.RewardEntities.Add(reward);
        }
        
        public void StartRewardCard()
        {
            var fsm = _container.Resolve<FsmGameplay>();
            var rewards = new RewardsProgress();
            rewards.Cards.Add(1, GetTower(rewards));
            rewards.Cards.Add(2, GetTower(rewards));
            rewards.Cards.Add(3, GetTower(rewards));
            fsm.Fsm.SetState<FsmStateBuildBegin>(rewards);
        }

        public RewardsProgress StartRewardProgress()
        {
            var rewards = new RewardsProgress();
            rewards.Cards.Add(1, GetTower(rewards));
            rewards.Cards.Add(2, GetTower(rewards));
            rewards.Cards.Add(3, GetTower(rewards));
            return rewards;
        }
        /**
         * По типу награды, который вернулся от игрока запускаем метод сервиса, передав данные.
         * Создаем награды и меняем состояние на начало строительства
         */
        public RewardsProgress GenerateRewardProgress()
        {
            var rewards = new RewardsProgress();
            //Тип наград
            // 1. Новая башня
            // 2. Апгрейд башни
            // 3. Участок земли
            // 4. Дорога
            // 5. Апгрейд навыка
            // 6. Апгрейд героя
            // 7. Перемещение башни
            // 8. Обмен местами
            // 9. Платформа для башни
            
            List<Func<RewardsProgress, RewardCardData>> getReward = new()
            {
                GetTower,
                GetTowerUpgrade,
                GetGround,
                GetRoad,
                //GetTowerMovement //TODO Добавить перемещения башен, после реализации
            };

            for (var i = 1; i <= 3; i++)
            {
                RewardCardData card = null;
                while (card == null)
                {
                    var typeReward = Mathf.FloorToInt(Mathf.Abs(Random.insideUnitSphere.x) * 999) % getReward.Count;
                    var func = getReward[typeReward]; //Получаем случайную ф-цию для расчета награды
                    card = func(rewards);
                    if (card == null) getReward.Remove(func); //Если тек.награду получить не можем, удаляем из списка, и ищем следующую

                    if (card == null && getReward.Count == 0) throw new Exception("Нет списка наград");
                }

                rewards.Cards.Add(i, card);
            }
            
            return rewards;
        }

        private RewardCardData GetRoad(RewardsProgress progress)
        {
            var listRoads = new List<string>();
            for (var i = 0; i < 9; i++)
            {
                listRoads.Add(i.ToString());
            }
            //Исключить повторения
            foreach (var progressCard in progress.Cards)
            {
                if (progressCard.Value.RewardType == RewardType.Road)
                {
                    listRoads.Remove(progressCard.Value.ConfigId);
                }
            }
            
            var number = Mathf.FloorToInt(Mathf.Abs(Random.insideUnitSphere.x) * 999) % listRoads.Count;
            var rewardId = listRoads[number];
            
            return new RewardCardData
            {
                RewardType = RewardType.Road,
                ConfigId = rewardId,
            };
        }

        private RewardCardData GetGround(RewardsProgress progress)
        {
            if (progress.Cards
                .Any(progressCard => progressCard.Value.RewardType == RewardType.Ground)
                )
                return null;
            

            return new RewardCardData
            {
                RewardType = RewardType.Ground,
            };
        }

        private RewardCardData GetTower(RewardsProgress progress)
        {
            var availableTowers = _towersService.GetAvailableTowers(); //Список доступных башен по колоде
            //Исключить повторения
            foreach (var rewardCardData in progress.Cards.Where(rewardCardData =>
                         rewardCardData.Value.RewardType == RewardType.Tower))
            {
                availableTowers.Remove(rewardCardData.Value.ConfigId);
            }

            var ran = new System.Random();
            var index = ran.Next(0, availableTowers.Count); //Случ.порядковый номер для строения
            var i = 0;
            foreach (var tower in availableTowers) //Перебираем список башен до случ.порядкового номера
            {
                if (i == index)
                {
                    //Получаем настройки башни
                    var towerSetting = _towersSettings.AllTowers.FirstOrDefault(t => t.ConfigId == tower.Key);
                    
                    if (towerSetting == null) throw new Exception("Не найдены настройки башни " + tower.Key);

                    return new RewardCardData
                    {
                        RewardType = RewardType.Tower,
                        ConfigId = tower.Key,
                        Level = _towersService.Levels[tower.Key], //текущий уровень, для звездочек
                        OnRoad = towerSetting.OnRoad,
                        EpicLevel = tower.Value,
                    };
                }
                i++;
            }

            return null;
        }

        private RewardCardData GetTowerUpgrade(RewardsProgress progress)
        {
            var availableUpgradeTowers = _towersService.GetAvailableUpgradeTowers(); //Список доступных улучшений
            //Исключить повторения
            foreach (var rewardCardData in progress.Cards.Where(rewardCardData =>
                         rewardCardData.Value.RewardType == RewardType.TowerLevelUp))
            {
                availableUpgradeTowers.Remove(rewardCardData.Value.ConfigId);
            }

            if (availableUpgradeTowers.Count == 0) return null;
            
            var index = Mathf.FloorToInt(Mathf.Abs(Random.insideUnitSphere.x) * 999) % availableUpgradeTowers.Count;

            var i = 0;
            foreach (var tower in availableUpgradeTowers)
            {
                if (i == index)
                {
                    return new RewardCardData
                    {
                        RewardType = RewardType.TowerLevelUp,
                        ConfigId = tower.Key,
                        Level = tower.Value, //текущий уровень, для звездочек
                    };
                }
                i++;
            }
            return null;
        }

        private RewardCardData GetTowerMovement(RewardsProgress progress)
        {
            var list = new List<RewardType>()
            {
                RewardType.TowerMove,
                RewardType.TowerReplace
            };

            foreach (var progressCard  in progress.Cards)
            {
                if (progressCard.Value.RewardType is RewardType.TowerMove or RewardType.TowerReplace)
                    return null;
            }
            
            var index = Mathf.FloorToInt(Mathf.Abs(Random.insideUnitSphere.x) * 999) % 2;
            
            return new RewardCardData
            {
                RewardType = list[index],
            };
        }
    }
}