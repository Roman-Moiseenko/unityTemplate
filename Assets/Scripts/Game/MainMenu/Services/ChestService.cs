using System;
using System.Collections;
using System.Collections.Generic;
using Game.Common;
using Game.GamePlay.Classes;
using Game.GameRoot.Commands.HardCurrency;
using Game.MainMenu.Commands.ChestCommands;
using Game.MainMenu.Root;
using Game.Settings;
using Game.Settings.Gameplay.Maps;
using Game.State.Inventory;
using Game.State.Inventory.Chests;
using Game.State.Maps.Rewards;
using Game.State.Root;
using MVVM.CMD;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using Scripts.Utils;
using UnityEngine;
using Random = System.Random;

namespace Game.MainMenu.Services
{
    public class ChestService
    {
        private GameStateProxy _gameState;
        private readonly ICommandProcessor _cmd;
        private readonly GameSettings _gameSettings;
        public ObservableDictionary<int, Chest> Chests;

        public ReactiveProperty<int> TimeLeft = new(0);
        public ReactiveProperty<int> CostLeft = new(0);
        private readonly Coroutines _coroutines;
        private Coroutine _opening;
        private ContainerChests _containerChests;
        public ReactiveProperty<int> CellOpening = new(0);

        public ChestService(GameStateProxy gameState,
            ICommandProcessor cmd,
            GameSettings gameSettings
        )
        {
            _gameState = gameState;
            _cmd = cmd;
            _gameSettings = gameSettings;
            _containerChests = _gameState.ContainerChests;

            CellOpening = _gameState.ContainerChests.CellOpening;
            CellOpening.Skip(1).Subscribe(newValue =>
            {
                _gameState.ContainerChests.StartOpening.Value =
                    newValue == 0 ? 0 : DateTime.Now.ToUniversalTime().ToFileTimeUtc();
            });
            Chests = _gameState.ContainerChests.Chests;
            _coroutines = GameObject.Find("[COROUTINES]").GetComponent<Coroutines>();

            _gameState.ContainerChests.CellOpening.Skip(1).Subscribe(cell =>
            {
                if (cell > 0)
                {
                    Debug.Log("Запуск открытия сундука = " + cell);
                    _opening = _coroutines.StartCoroutine(ChestOpening());
                }
                else
                {
                    if (_opening != null) _coroutines.StopCoroutine(_opening);
                }
            });
//            TimeLeft.Subscribe(v => Debug.Log("v = " + v));
            //Проверка, если время на открытие вышло
            TimeLeft.Skip(1).Where(x => x == 0).Subscribe(_ =>
            {
                if (_containerChests.CellOpening.CurrentValue == 0) return;
                //  Debug.Log("Время вышло открываем = " + _containerChests.CellOpening.CurrentValue);
                var command = new CommandChestOpened();
                _cmd.Process(command);
            });
        }

        private IEnumerator ChestOpening()
        {
            //Проверка на окончание времени
            if (!CheckOpeningChest()) yield break;
//            Debug.Log("Проверка на ChestOpening");
            yield return new WaitForSeconds(60);
            _coroutines.StartCoroutine(ChestOpening());
        }

        /**
         * Проверка при запуске на открытые сундуки
         */
        public void StartOpeningChests()
        {
            if (CheckOpeningChest())
            {
                _opening = _coroutines.StartCoroutine(ChestOpening());
            }
        }

        /**
         * Проверяем и сохраняем время оставшееся на открытие
         */
        private bool CheckOpeningChest()
        {
            if (_containerChests.CellOpening.CurrentValue == 0) return false;

            var tNow = DateTime.Now.ToUniversalTime();
            var tStart = DateTime.FromFileTimeUtc(_containerChests.StartOpening.CurrentValue);
            var span = tNow - tStart;

            var fullMinutes = Chests[_containerChests.CellOpening.CurrentValue].TypeChest.FullHourOpening() * 60;
            var result = fullMinutes - (int)span.TotalMinutes;
            TimeLeft.OnNext(result < 0 ? 0 : result);
            CostLeft.OnNext((int)(TimeLeft.Value * AppConstants.RATIO_COST_OPEN_CHEST));
            return result > 0;
        }


        /**
         * Предварительный список возможных наград из Сундука
         */

        //TODO Переделать список наград из Сундука
        public List<RewardEntityData> GetListRewards(Chest chest)
        {
            if (chest.Gameplay == TypeGameplay.Levels)
            {
                var rewardSettings = _gameSettings.MapsSettings.Maps.Find(v => v.MapId == chest.MapId).MapRewardSetting;
                return rewardSettings.RewardChest[chest.TypeChest];
            }

            throw new Exception($"Не задана настройка для {chest.Gameplay}");

            /*

            var list = new Dictionary<InventoryType, Dictionary<string, int>>();
            //TODO Награды Infinity
            const int baseGoldRatio = 1000;
            //1 Gold
            var goldAmount = chest.Level * chest.TypeChest.GetRatioReward() * baseGoldRatio;
            var gold = new Dictionary<string, int>();
            gold.Add("Currency", goldAmount);
            list.Add(InventoryType.SoftCurrency, gold);
            //Чертежи Башен
            var towerPlanAmount = chest.Level * chest.TypeChest.GetRatioReward();
            var towerPlan = new Dictionary<string, int>();
            towerPlan.Add("UnKnow", towerPlanAmount);
            list.Add(InventoryType.TowerPlan, towerPlan);
            return list;*/
        }

        public List<RewardEntityData> GetListChanceRewards(Chest chest)
        {
            if (chest.Gameplay == TypeGameplay.Levels)
            {
                var rewardSettings = _gameSettings.MapsSettings.Maps.Find(v => v.MapId == chest.MapId).MapRewardSetting;
                return rewardSettings.RewardChanceChest[chest.TypeChest];
            }

            throw new Exception($"Не задана настройка для {chest.Gameplay}");

            /*

            var list = new Dictionary<InventoryType, Dictionary<string, int>>();
            //TODO Награды Infinity
            const int baseGoldRatio = 1000;
            //1 Gold
            var goldAmount = chest.Level * chest.TypeChest.GetRatioReward() * baseGoldRatio;
            var gold = new Dictionary<string, int>();
            gold.Add("Currency", goldAmount);
            list.Add(InventoryType.SoftCurrency, gold);
            //Чертежи Башен
            var towerPlanAmount = chest.Level * chest.TypeChest.GetRatioReward();
            var towerPlan = new Dictionary<string, int>();
            towerPlan.Add("UnKnow", towerPlanAmount);
            list.Add(InventoryType.TowerPlan, towerPlan);
            return list;*/
        }

        /**
        * Готовый список возможных наград из Сундука
        */
        public List<RewardEntityData> GenerateRewards(Chest chest)
        {
            var rewards = GetListRewards(chest);
            var chanceRewards = GetListChanceRewards(chest);
            var result = new List<RewardEntityData>();
            var random = new Random();
            //Список доступных башен для текущей карты
            var listConfig = new List<string>();
            foreach (var towerSettings in _gameSettings.TowersSettings.AllTowers)
            {
                if (towerSettings.AvailableWave <= chest.MapId)
                {
                    listConfig.Add(towerSettings.ConfigId);
                }
            }


            //Переносим награды в список на выдачу
            foreach (var reward in rewards)
            {
                var newReward = new RewardEntityData();
                newReward.RewardType = reward.RewardType;
                newReward.Amount = reward.Amount;
                
                if (reward.RewardType is InventoryType.TowerCard or InventoryType.TowerPlan)
                {
                    newReward.ConfigId = (reward.ConfigId != "") 
                        ? reward.ConfigId 
                        : listConfig[random.Next(listConfig.Count)];
                }
                result.Add(newReward);
                
            }

            //TODO ВОзможные награды добавить
/*
            //Добавляем возможные награды в список на выдачу
            foreach (var chanceReward in chanceRewards)
            {
                var chance = random.Next(100);

                if (chance % chanceReward.Type.GetChanceRandom() == 0)
                {
                    rewards.Find(v => )
                }
            }
*/
            return result;
/*
            var list = new Dictionary<InventoryType, Dictionary<string, int>>();

            //TODO Запускаем команды сохранения наград
            const int baseGoldRatio = 1000;
            //1 Gold
            var goldAmount = chest.Level * chest.TypeChest.GetRatioReward() * baseGoldRatio;
            var gold = new Dictionary<string, int>();
            gold.Add("Currency", goldAmount);
            list.Add(InventoryType.SoftCurrency, gold);
            //Чертежи Башен
            var towerPlanAmount = chest.Level * chest.TypeChest.GetRatioReward();
            //TODO Берем из доступных чертежей все Конфиги, распределяем случайное кол-во наград

            var listConfig = new List<string>();
            foreach (var towerSettings in _gameSettings.TowersSettings.AllTowers)
            {
                if (towerSettings.AvailableWave <= chest.Wave)
                {
                    listConfig.Add(towerSettings.ConfigId);
                }
            }

            var AllTotal = listConfig.Count;
            var totalIem = towerPlanAmount / AllTotal;

            var index = 0;
            var towerPlan = new Dictionary<string, int>();
            foreach (var configId in listConfig)
            {
                index++;
                if (index == AllTotal)
                {
                    towerPlan.Add(configId, towerPlanAmount - (index - 1) * totalIem);
                }
                else
                {
                    towerPlan.Add(configId, totalIem);
                }
            }

            list.Add(InventoryType.TowerPlan, towerPlan);


            return list;
            */
        }

        /**
         * Команда запуск открытия сундука
         */
        public void OpeningChest(Chest chest)
        {
            var command = new CommandChestOpening
            {
                Cell = chest.Cell
            };
            _cmd.Process(command);
        }

        /**
         * Добавляем сундук из Бесконечного боя
         */
        public TypeChest AddChestInfinity(int lastWave)
        {
            var levelChest = lastWave / 100 + 1;
            var epic = lastWave switch
            {
                100 => TypeChest.Legend,
                > 60 => TypeChest.Epic,
                > 30 => TypeChest.Gold,
                _ => TypeChest.Silver
            };

            var command = new CommandChestAdd
            {
                Gameplay = TypeGameplay.Infinity,
                TypeChest = epic,
                LevelChest = levelChest,
                Wave = lastWave,
            };
            return _cmd.Process(command) ? epic : TypeChest.None;
        }
        
        
        public bool AddChestLevel(MainMenuEnterParams enterParams)
        {
            var command = new CommandChestAdd
            {
                Gameplay = TypeGameplay.Levels,
                TypeChest = enterParams.TypeChest,
                LevelChest = 0,
                Wave = enterParams.LastWave,
                MapId = enterParams.MapId,
            };
            
            return !_cmd.Process(command);
        }


        public void ForcedCurrentChest(Chest chest)
        {
            var command = new CommandChestForced();
            _cmd.Process(command);
            CheckOpeningChest();
        }

        /**
         * Открытие сундука, который Closed или Opening За кристалы
         */
        public List<RewardEntityData> OpenCurrentChest(Chest chest)
        {
            var timeLeft = 0;

            if (chest.Status.Value == StatusChest.Close)
            {
                timeLeft = chest.TypeChest.FullHourOpening() * 60;
            }

            if (chest.Status.Value == StatusChest.Opening)
            {
                timeLeft = TimeLeft.CurrentValue;
                TimeLeft.OnNext(0);
                CellOpening.OnNext(0);
            }

            if (timeLeft == 0) return null;

            var cost = (int)(timeLeft * AppConstants.RATIO_COST_OPEN_CHEST);
            var commandHard = new CommandSpendHardCurrency(cost);
            _cmd.Process(commandHard);


            return CommandRewardOpenChest(chest);
        }

        /**
         * Открытие сундука, который Opened готов к открытию бесплатно
         */
        public List<RewardEntityData> OpenCompletedChest(Chest chest)
        {
            if (chest.Status.Value != StatusChest.Opened) return null;
            return CommandRewardOpenChest(chest);
        }

        private List<RewardEntityData> CommandRewardOpenChest(Chest chest)
        {
            chest.Status.OnNext(StatusChest.Open); //Переводим в статус Открывают для наград - подписки

            //Генерируем награды
            var reward = GenerateRewards(chest);
            //Debug.Log(JsonConvert.SerializeObject(reward, Formatting.Indented));
            //Команда открытия сундука с наградами (сохраняем награды, удаляем сундук)
            var command = new CommandChestOpen();
            command.Rewards = reward;
            command.Cell = chest.Cell;
            _cmd.Process(command);

            return reward;
        }

    }
}