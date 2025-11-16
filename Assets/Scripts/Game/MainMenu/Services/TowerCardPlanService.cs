using System;
using System.Collections.Generic;
using System.Linq;
using DI;
using Game.GameRoot.Commands;
using Game.MainMenu.Commands.InventoryCommands;
using Game.MainMenu.Commands.SoftCurrency;
using Game.MainMenu.Commands.TowerCommands;
using Game.MainMenu.View;
using Game.MainMenu.View.ScreenInventory.PopupBlacksmith.PrefabBinders;
using Game.MainMenu.View.ScreenInventory.TowerCards;
using Game.MainMenu.View.ScreenInventory.TowerPlans;
using Game.Settings.Gameplay.Entities.Tower;
using Game.State;
using Game.State.Inventory;
using Game.State.Inventory.Deck;
using Game.State.Inventory.TowerCards;
using Game.State.Inventory.TowerPlans;
using Game.State.Maps.Towers;
using MVVM.CMD;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.MainMenu.Services
{
    public class TowerCardPlanService
    {
        private readonly IObservableCollection<InventoryItem> _items; //кешируем
        private readonly InventoryRoot _inventoryRoot;
        private readonly ICommandProcessor _cmd;
        private readonly DIContainer _container;

        private readonly ObservableList<TowerCardViewModel> _allTowerCards = new();
        private readonly ObservableList<TowerPlanViewModel> _allTowerPlans = new();
        private readonly Dictionary<int, TowerCardViewModel> _towerCardsMap = new();
        private readonly Dictionary<int, TowerPlanViewModel> _towerPlansMap = new();

        private readonly Dictionary<string, TowerSettings> _towerSettingsMap = new();
        private readonly DeckCard _currentDeck;
        
        public IObservableCollection<TowerCardViewModel> AllTowerCards =>
            _allTowerCards; //Интерфейс менять нельзя, возвращаем через динамический массив

        public IObservableCollection<TowerPlanViewModel> AllTowerPlans =>
            _allTowerPlans; //Интерфейс менять нельзя, возвращаем через динамический массив

        public TowerCardPlanService(
            InventoryRoot inventoryRoot,
            TowersSettings towersSettings,
            ICommandProcessor cmd,
            DIContainer container
        )
        {
       //     var gameStateProvider = container.Resolve<IGameStateProvider>(); //Получаем репозиторий
         //   var gameState = gameStateProvider.GameState;
            
            _inventoryRoot = inventoryRoot;
            _items = inventoryRoot.Items;
            _cmd = cmd;
            _container = container;
            _currentDeck = _inventoryRoot.GetCurrentDeckCard();

            //Кешируем настройки зданий / обектов
            foreach (var towerSettings in towersSettings.AllTowers)
            {
                _towerSettingsMap[towerSettings.ConfigId] = towerSettings;
            }
        //    Debug.Log(JsonConvert.SerializeObject(_towerSettingsMap, Formatting.Indented));

         //   Debug.Log(JsonConvert.SerializeObject(_items, Formatting.Indented));

            foreach (var item in _items)
            {
                if (item is TowerCard towerCard)
                {
                    towerCard.EpicLevel.Skip(1).Subscribe(e => UpdateParameterTowerCard(towerCard));
                    towerCard.Level.Subscribe(e => UpdateParameterTowerCard(towerCard));
                    UpdateParameterTowerCard(towerCard);
                    CreateTowerCardViewModel(towerCard);

                }

                if (item is TowerPlan towerPlan)
                {
                    CreateTowerPlanViewModel(towerPlan);
                }
            }
            //Debug.Log(JsonConvert.SerializeObject(_items, Formatting.Indented));
        //    Debug.Log(JsonConvert.SerializeObject(gameState.Inventory.Items, Formatting.Indented));

            var command = new CommandSaveGameState();
            cmd.Process(command);
            _items.ObserveAdd().Subscribe(e =>
            {
                if (e.Value is TowerCard towerCard)
                {
                    towerCard.EpicLevel.Subscribe(_ => UpdateParameterTowerCard(towerCard));
                    towerCard.Level.Subscribe(_ => UpdateParameterTowerCard(towerCard));
                    CreateTowerCardViewModel(towerCard); //Создаем View Model
                }

                if (e.Value is TowerPlan towerPlan)
                {
                    CreateTowerPlanViewModel(towerPlan);
                }
            });
            //Если у сущности изменился уровень, меняем его и во вью-модели
            _items.ObserveRemove().Subscribe(e =>
            {
                if (e.Value is TowerCard towerCard) RemoveTowerCardViewModel(towerCard);
                if (e.Value is TowerPlan towerPlan) RemoveTowerPlanViewModel(towerPlan);
            });

            foreach (var deckTowerCardId in _currentDeck.TowerCardIds)
            {
                ChangeDeckTowerCardViewModel(deckTowerCardId.Value);
            }

            _currentDeck.TowerCardIds.ObserveAdd().Subscribe(e => { ChangeDeckTowerCardViewModel(e.Value.Value); });
            _currentDeck.TowerCardIds.ObserveRemove().Subscribe(e => { ChangeDeckTowerCardViewModel(e.Value.Value); });
            //  Debug.Log(JsonConvert.SerializeObject(_allTowerCards[3].NumberCardDeck, Formatting.Indented));
        }

        public void ChangeDeckTowerCard(int uniqueId)
        {
            var towerView = _allTowerCards.FirstOrDefault(t => t.IdTowerCard == uniqueId)!;

            if (_currentDeck.TowerCardInDeck(uniqueId))
            {
                _currentDeck.ExtractFromDeck(uniqueId);
                towerView.NumberCardDeck = 0;
                towerView.IsDeck.OnNext(false);
            }
            else
            {
                towerView.NumberCardDeck = _currentDeck.PushToDeck(uniqueId);
                towerView.IsDeck.OnNext(true);
            }

            var command = new CommandSaveGameState();
            _cmd.Process(command);
        }

        private void ChangeDeckTowerCardViewModel(int uniqueId)
        {
            var towerView = _allTowerCards.FirstOrDefault(t => t.IdTowerCard == uniqueId)!;
            towerView.NumberCardDeck = 0;
            foreach (var towerCardId in _currentDeck.TowerCardIds)
            {
                if (towerCardId.Value == uniqueId)
                {
                    towerView.NumberCardDeck = towerCardId.Key;
                    towerView.IsDeck.Value = true;
                    return;
                }
            }

            towerView.IsDeck.Value = false;
        }

        //ПУБЛИЧНЫЕ МЕТОДЫ ИЗМЕНЕНИЯ TowerCardEntity

        public void LevelUpTowerCard(int uniqueId)
        {
            var towerCardEntity = _inventoryRoot.Get<TowerCard>(uniqueId);
            var towerCard = _inventoryRoot.Get<TowerCard>(uniqueId);
            var commandCard = new CommandTowerCardLevelUp(uniqueId);
            var currency = towerCardEntity.GetCostCurrencyLevelUpTowerCard();
            var plan = towerCardEntity.GetCostPlanLevelUpTowerCard();

            var item = _inventoryRoot.GetByConfigAndType<TowerPlan>(InventoryType.TowerPlan, towerCard.ConfigId);
            var commandPlan = new CommandInventoryItemSpend(item.UniqueId, plan);

            var commandCurrency = new CommandSoftCurrencySpend(currency);
            _cmd.Process(commandPlan);
            _cmd.Process(commandCard);
            _cmd.Process(commandCurrency);
        }

        private void CreateTowerCardViewModel(TowerCard towerCard)
        {
            var towerCardViewModel = new TowerCardViewModel(
                towerCard,
                _towerSettingsMap[towerCard.ConfigId],
                this,
                _container
            ); //3

            //TODO Проверить находится ли карта в колоде
            _allTowerCards.Add(towerCardViewModel); //4
            _towerCardsMap[towerCard.UniqueId] = towerCardViewModel;
        }

        /**
         * Удаляем объект из списка моделей и из кеша
         */
        private void RemoveTowerCardViewModel(TowerCard towerCard)
        {
            if (_towerCardsMap.TryGetValue(towerCard.UniqueId, out var towerCardViewModel))
            {
                _allTowerCards.Remove(towerCardViewModel);
                _towerCardsMap.Remove(towerCard.UniqueId);
            }
        }

        private void CreateTowerPlanViewModel(TowerPlan towerPlan)
        {
            var towerPlanViewModel = new TowerPlanViewModel(towerPlan,
                _towerSettingsMap[towerPlan.ConfigId],
                _container);
            _allTowerPlans.Add(towerPlanViewModel); //4
            _towerPlansMap[towerPlan.UniqueId] = towerPlanViewModel;
        }

        private void RemoveTowerPlanViewModel(TowerPlan towerPlan)
        {
            if (_towerPlansMap.TryGetValue(towerPlan.UniqueId, out var towerPlanViewModel))
            {
                _allTowerPlans.Remove(towerPlanViewModel);
                _towerPlansMap.Remove(towerPlan.UniqueId);
            }
        }

        /**
         * Пересчитываем параметры башни в зависимости от уровня и эпичности
         */
        public void UpdateParameterTowerCard(TowerCard towerCard)
        {
            //    var epic = towerCard.EpicLevel.Value;
            //    var level = towerCard.Level.Value;

            var settings = _towerSettingsMap[towerCard.ConfigId];
            

            towerCard.Parameters.Clear();
            foreach (var baseParameter in settings.BaseParameters)
            {
                towerCard.Parameters.Add(baseParameter.ParameterType, new TowerParameter(new TowerParameterData(baseParameter)));
            }
            
            //Debug.Log(JsonConvert.SerializeObject(towerCard.Parameters, Formatting.Indented));

            //TODO Пересчет от базовых параметров 
            foreach (var keyValue in towerCard.Parameters)
            {
                //  Debug.Log(keyValue.Key);
                var towerParam = keyValue.Value;
                //Возвращаем базовое значение
                towerParam.Value.Value =
                    settings.BaseParameters.FirstOrDefault(p => p.ParameterType == towerParam.ParameterType)!.Value;
                //   Debug.Log("towerParam.Value.CurrentValue = " + towerParam.Value.CurrentValue);

                //   Debug.Log(towerCard.EpicLevel.CurrentValue);
                //Обсчет эпичности
                var epicCardParam = settings.EpicCardParameters.Find(p => p.ParameterType == towerParam.ParameterType);
                if (epicCardParam != null)
                {
//                    Debug.Log(JsonConvert.SerializeObject(epicCardParam, Formatting.Indented));
                    foreach (var epicParameter in epicCardParam.EpicParameters)
                    {
                        if (towerCard.EpicLevel.CurrentValue.Index() < epicParameter.Level.Index()) break;
                        //      Debug.Log(epicParameter.Level);
                        towerParam.Value.Value *= (1 + epicParameter.Percent / 100);
                        //      Debug.Log("towerParam.Value.CurrentValue = " + towerParam.Value.CurrentValue);
                        if (epicParameter.Level == towerCard.EpicLevel.CurrentValue) break;
                    }
                }

                //    Debug.Log("2 towerParam.Value.CurrentValue = " + towerParam.Value.CurrentValue);
                //Увеличиваем значения от уровня карты, если параметр изменчив
                var levelParam = settings.LevelCardParameters.Find(p => p.ParameterType == towerParam.ParameterType);
                if (levelParam != null)
                {
                    // Debug.Log(JsonConvert.SerializeObject(levelParam, Formatting.Indented));

                    var rateEpic = Mathf.Pow(levelParam.PowEpic, towerCard.EpicLevel.Value.Index()); //Коэф.роста
                    //         Debug.Log("rateEpic = " + rateEpic);
                    //         Debug.Log("towerCard.Level.Value = " + towerCard.Level.Value);
                    towerParam.Value.Value += rateEpic * levelParam.BaseValue * (towerCard.Level.Value - 1);
                    //          Debug.Log("towerParam.Value.CurrentValue = " + towerParam.Value.CurrentValue);
                }
                /*
                if (towerCard.Parameters.TryGetValue(keyValue.Key, out var towerParameter))
                {
                    //Возвращаем базовое значение
                    towerParameter.Value.Value = settings.BaseParameters.FirstOrDefault(
                        param => param.ParameterType == keyValue.Key
                    )!.Value;
                    //Проходим все эпичные уровни и обсчитываем значения

                    foreach (TypeEpicCard typeEpic in Enum.GetValues(typeof(TypeEpicCard)))
                    {
                        //Получаем настройки для уровня typeEpic
                        var epicSettings = settings.EpicLevels.FirstOrDefault(e => e.Level == typeEpic);

                        var epicUpgrade =
                            epicSettings?.UpgradeParameters.FirstOrDefault(e => e.ParameterType == keyValue.Key);
                        if (epicUpgrade != null)
                        {
                            //Debug.Log("epicUpgrade.Value  =  " + epicUpgrade.Value);
                            towerParameter.Value.Value =
                                towerParameter.Value.CurrentValue * (1 + epicUpgrade.Value / 100);
                        }

                        if (typeEpic == epic) break; //Совпал уровень эпичности, следующие не грузим
                    }

                    //Рассчитываем для уровня Level


                    var levelUpgrade = settings.EpicLevels.FirstOrDefault(e => e.Level == epic)?.LevelCardParameters
                        .FirstOrDefault(e => e.ParameterType == keyValue.Key);
                    if (levelUpgrade != null)
                    {
                        towerParameter.Value.Value += levelUpgrade.Value * (level - 1);
                    }
                }

*/
            }

            //     Debug.Log("UpdateParameterTowerCard для " + towerCard.UniqueId + $" ({towerCard.ConfigId})");
            //      Debug.Log(JsonConvert.SerializeObject(towerCard.Parameters, Formatting.Indented));
        }


        public InfoUpgradedViewModel GetInfoUpgradedViewModel(string configId,
            TypeEpicCard epicLevel, int level)
        {
            var viewModel = new InfoUpgradedViewModel();
            var towerSetting = _towerSettingsMap[configId];
            var maxLevel = epicLevel.MaxLevel();
            viewModel.NameTower = towerSetting.TitleLid;
            viewModel.NameEpic = epicLevel.GetString();

            var towerCardData = new TowerCardData
            {
                EpicLevel = epicLevel,
                ConfigId = configId,
                Level = level,
                Parameters = new Dictionary<TowerParameterType, TowerParameterData>(),
                Defence = towerSetting.Defence,
            };
            foreach (var baseParameter in towerSetting.BaseParameters)
            {
                towerCardData.Parameters.Add(baseParameter.ParameterType, new TowerParameterData(baseParameter));
            }

            var towerCard = new TowerCard(towerCardData);
            towerCardData.EpicLevel = towerCardData.EpicLevel.Next();
            var towerCardAfter = new TowerCard(towerCardData);

            UpdateParameterTowerCard(towerCard);
            UpdateParameterTowerCard(towerCardAfter);

            viewModel.Parameters.Add(
                "МАКС.УРОВЕНЬ",
                new Vector2(towerCard.EpicLevel.Value.MaxLevel(), towerCardAfter.EpicLevel.Value.MaxLevel())
            );

            foreach (var valuePair in towerCard.Parameters)
            {
                if (!towerCardAfter.Parameters.TryGetValue(valuePair.Key, out var valueAfter))
                    throw new Exception("Ошибка");

                if (!Mathf.Approximately(valuePair.Value.Value.CurrentValue, valueAfter.Value.CurrentValue))
                    viewModel.Parameters.Add(valuePair.Key.GetString(),
                        new Vector2(valuePair.Value.Value.CurrentValue, valueAfter.Value.CurrentValue));
            }

            return viewModel;
        }
    }
}