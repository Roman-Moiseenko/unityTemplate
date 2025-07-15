using System;
using System.Collections.Generic;
using System.Linq;
using DI;
using Game.GamePlay.Commands.TowerCommand;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.States;
using Game.Settings;
using Game.Settings.Gameplay.Entities.Tower;
using Game.State;
using Game.State.Gameplay;
using Game.State.Maps.Towers;
using MVVM.CMD;
using Newtonsoft.Json;
using R3;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.GamePlay.Services
{
    public class RewardProgressService
    {
        private readonly DIContainer _container;
        private readonly TowersSettings _towersSettings;
        private readonly TowersService _towerService;
        private readonly GroundsService _groundService;

        public RewardProgressService(
            DIContainer container,
            TowersSettings towersSettings
            //TODO Добавить SkillsSettings, HeroesSettings
        )
        {
            _container = container;
            //Сервисы для наград
            _towerService = container.Resolve<TowersService>();
            _groundService = container.Resolve<GroundsService>();

            var gameplayStateProxy = container.Resolve<IGameStateProvider>().GameplayState;
            _towersSettings = towersSettings;


            var fsm = container.Resolve<FsmGameplay>();

            gameplayStateProxy.Progress.Subscribe(newValue =>
            {
                if (newValue < 100) return;
                
                var rewards = GenerateReward(); //1. Создаем награды
                fsm.Fsm.SetState<FsmStateBuildBegin>(rewards);
            });

            //TODO Куда перенести
            fsm.Fsm.StateCurrent.Subscribe(newState =>
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
                    if (gameplayStateProxy.Progress.Value >= 100) gameplayStateProxy.Progress.Value -= 100;
                    gameplayStateProxy.ProgressLevel.Value++;
                }
            });
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

        /**
         * По типу награды, который вернулся от игрока запускаем метод сервиса, передав данные
         */
        private RewardsProgress GenerateReward()
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

            var text = rewardId switch
            {
                "0" or "1" => "1X1",
                "8" => "2X2",
                _ => "1X2"
            };

            return new RewardCardData
            {
                RewardType = RewardType.Road,
                ConfigId = rewardId,
                Description = "Дорога " + text
            };
        }

        private RewardCardData GetGround(RewardsProgress progress)
        {
            foreach (var progressCard in progress.Cards)
            {
                if (progressCard.Value.RewardType == RewardType.Ground)
                    return null;
            }
            
            return new RewardCardData
            {
                RewardType = RewardType.Ground,
                Description = "ЗЕМЛЯ"
            };
        }

        private RewardCardData GetTower(RewardsProgress progress)
        {
            var availableTowers = _towerService.GetAvailableTowers(); //Список доступных башен по колоде
            //Исключить повторения
            foreach (var rewardCardData in progress.Cards.Where(rewardCardData =>
                         rewardCardData.Value.RewardType == RewardType.Tower))
            {
                availableTowers.Remove(rewardCardData.Value.ConfigId);
            }

            var index = Mathf.FloorToInt(Mathf.Abs(Random.insideUnitSphere.x) * 999) %
                        availableTowers.Count; //Случ.порядковый номер для строения
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
                        RewardLevel = tower.Value, //текущий уровень, для звездочек
                        Name = towerSetting.name, //Название башни
                        Description = "Башня " + towerSetting.name
                    };
                }

                i++;
            }

            return null;
        }

        private RewardCardData GetTowerUpgrade(RewardsProgress progress)
        {
            var availableUpgradeTowers = _towerService.GetAvailableUpgradeTowers(); //Список доступных улучшений
            //Исключить повторения
            foreach (var rewardCardData in progress.Cards.Where(rewardCardData =>
                         rewardCardData.Value.RewardType == RewardType.TowerLevelUp))
            {
                availableUpgradeTowers.Remove(rewardCardData.Value.ConfigId);
            }


            var index = Mathf.FloorToInt(Mathf.Abs(Random.insideUnitSphere.x) * 999) % availableUpgradeTowers.Count;

            var i = 0;
            foreach (var tower in availableUpgradeTowers)
            {
                if (i == index)
                {
                    var towerSetting = _towersSettings.AllTowers.FirstOrDefault(t => t.ConfigId == tower.Key);
                    if (towerSetting == null) throw new Exception("Не найдены настройки башни " + tower.Key);
                    
                    var listParameters = towerSetting.GameplayLevels[tower.Value].Parameters;
                    var textDescription = "";
                    foreach (var parameter in listParameters)
                    {
                        if (textDescription != "") textDescription += "\n";
                        textDescription += parameter.Value + "% " + parameter.ParameterType.GetString();
                    }
                    
                    return new RewardCardData
                    {
                        RewardType = RewardType.TowerLevelUp,
                        ConfigId = tower.Key,
                        RewardLevel = tower.Value, //текущий уровень, для звездочек
                        Name = towerSetting.name, //Название башни
                        Description = textDescription,
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