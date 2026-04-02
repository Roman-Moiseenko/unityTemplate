using System;
using System.Collections.Generic;
using System.Linq;
using DI;
using Game.GameRoot.Commands;
using Game.MainMenu.Commands.InventoryCommands;
using Game.MainMenu.Commands.SkillCommands;
using Game.MainMenu.Commands.SoftCurrency;
using Game.MainMenu.View.ScreenInventory.PopupBlacksmith.PrefabBinders;
using Game.MainMenu.View.ScreenInventory.SkillCards;
using Game.MainMenu.View.ScreenInventory.SkillPlans;
using Game.Settings.Gameplay.Entities.Skill;
using Game.State.Common;
using Game.State.Inventory.Common;
using Game.State.Inventory.Deck;
using Game.State.Inventory.SkillCards;
using Game.State.Inventory.SkillPlans;
using Game.State.Maps.Skills;
using MVVM.CMD;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.MainMenu.Services
{
    public class SkillCardPlanService
    {
        private readonly IObservableCollection<InventoryItem> _items; //кешируем
        private readonly InventoryRoot _inventoryRoot;
        private readonly ICommandProcessor _cmd;
        private readonly DIContainer _container;

        private readonly ObservableList<SkillCardViewModel> _allSkillCards = new();
        private readonly ObservableList<SkillPlanViewModel> _allSkillPlans = new();
        private readonly Dictionary<int, SkillCardViewModel> _skillCardsMap = new();
        private readonly Dictionary<int, SkillPlanViewModel> _skillPlansMap = new();

        private readonly Dictionary<string, SkillSettings> _skillSettingsMap = new();
        private readonly DeckCard _currentDeck;
        
        public IObservableCollection<SkillCardViewModel> AllSkillCards =>
            _allSkillCards; //Интерфейс менять нельзя, возвращаем через динамический массив

        public IObservableCollection<SkillPlanViewModel> AllSkillPlans =>
            _allSkillPlans; //Интерфейс менять нельзя, возвращаем через динамический массив
        public SkillCardPlanService(
            InventoryRoot inventoryRoot,
            SkillsSettings skillsSettings,
            ICommandProcessor cmd,
            DIContainer container)
        {
            _inventoryRoot = inventoryRoot;
            _items = inventoryRoot.Items;
            _cmd = cmd;
            _container = container;
            _currentDeck = _inventoryRoot.GetCurrentDeckCard();
            
            //Кешируем настройки навыков
            foreach (var skillSettings in skillsSettings.AllSkills)
            {
                _skillSettingsMap[skillSettings.ConfigId] = skillSettings;
            }
            
            foreach (var item in _items)
            {
                if (item is SkillCard skillCard)
                {
                    skillCard.EpicLevel.Skip(1).Subscribe(e => UpdateParameterSkillCard(skillCard));
                    skillCard.Level.Subscribe(e => UpdateParameterSkillCard(skillCard));
                    UpdateParameterSkillCard(skillCard);
                    CreateSkillCardViewModel(skillCard);

                }

                if (item is SkillPlan skillPlan)
                {
                    CreateSkillPlanViewModel(skillPlan);
                }
            }
            
            var command = new CommandSaveGameState();
            cmd.Process(command);
            _items.ObserveAdd().Subscribe(e =>
            {
                if (e.Value is SkillCard skillCard)
                {
                    skillCard.EpicLevel.Subscribe(_ => UpdateParameterSkillCard(skillCard));
                    skillCard.Level.Subscribe(_ => UpdateParameterSkillCard(skillCard));
                    CreateSkillCardViewModel(skillCard); //Создаем View Model
                }

                if (e.Value is SkillPlan skillPlan)
                {
                    CreateSkillPlanViewModel(skillPlan);
                }
            });
            //Если у сущности изменился уровень, меняем его и во вью-модели
            _items.ObserveRemove().Subscribe(e =>
            {
                if (e.Value is SkillCard skillCard) RemoveSkillCardViewModel(skillCard);
                if (e.Value is SkillPlan skillPlan) RemoveSkillPlanViewModel(skillPlan);
            });

            foreach (var deckSkillCardId in _currentDeck.SkillCardIds)
            {
                ChangeDeckSkillViewModel(deckSkillCardId);
            }

            _currentDeck.SkillCardIds.ObserveAdd().Subscribe(e => { ChangeDeckSkillViewModel(e.Value); });
            _currentDeck.SkillCardIds.ObserveRemove().Subscribe(e => { ChangeDeckSkillViewModel(e.Value); });
        }
        
        
        public void ChangeDeckSkillCard(int uniqueId)
        {
            var skillView = _allSkillCards.FirstOrDefault(t => t.IdSkillCard == uniqueId)!;

            if (_currentDeck.SkillCardInDeck(uniqueId))
            {
                _currentDeck.ExtractSkillFromDeck(uniqueId);
                skillView.NumberCardDeck = 0;
                skillView.IsDeck.OnNext(false);
            }
            else
            {
                //TODO Проверка на ConfigId
                
                if (_currentDeck.PushSkillToDeck(uniqueId)) skillView.IsDeck.OnNext(true);
            }

            var command = new CommandSaveGameState();
            _cmd.Process(command);
        }

        private void ChangeDeckSkillViewModel(int uniqueId)
        {
            var skillView = _allSkillCards.FirstOrDefault(t => t.IdSkillCard == uniqueId)!;
            
            foreach (var skillCardId in _currentDeck.SkillCardIds)
            {
                if (skillCardId == uniqueId)
                {
                    skillView.IsDeck.Value = true;
                    return;
                }
            }

            skillView.IsDeck.Value = false;
        }

        //ПУБЛИЧНЫЕ МЕТОДЫ ИЗМЕНЕНИЯ SkillCardEntity

        public void LevelUpSkillCard(int uniqueId)
        {
            var skillCardEntity = _inventoryRoot.Get<SkillCard>(uniqueId);
            var skillCard = _inventoryRoot.Get<SkillCard>(uniqueId);
            var commandCard = new CommandSkillCardLevelUp(uniqueId);
            var currency = skillCardEntity.GetCostCurrencyLevelUpSkillCard();
            var plan = skillCardEntity.GetCostPlanLevelUpSkillCard();

            var item = _inventoryRoot.GetByConfigAndType<SkillPlan>(InventoryType.SkillPlan, skillCard.ConfigId);
            var commandPlan = new CommandInventoryItemSpend(item.UniqueId, plan);

            var commandCurrency = new CommandSoftCurrencySpend(currency);
            _cmd.Process(commandPlan);
            _cmd.Process(commandCard);
            _cmd.Process(commandCurrency);
        }

        private void CreateSkillCardViewModel(SkillCard skillCard)
        {
            var skillCardViewModel = new SkillCardViewModel(
                skillCard,
                _skillSettingsMap[skillCard.ConfigId],
                this,
                _container
            ); //3

            //TODO Проверить находится ли карта в колоде
            _allSkillCards.Add(skillCardViewModel); //4
            _skillCardsMap[skillCard.UniqueId] = skillCardViewModel;
        }

        /**
         * Удаляем объект из списка моделей и из кеша
         */
        private void RemoveSkillCardViewModel(SkillCard skillCard)
        {
            if (_skillCardsMap.TryGetValue(skillCard.UniqueId, out var skillCardViewModel))
            {
                _allSkillCards.Remove(skillCardViewModel);
                _skillCardsMap.Remove(skillCard.UniqueId);
            }
        }

        private void CreateSkillPlanViewModel(SkillPlan skillPlan)
        {
            var skillPlanViewModel = new SkillPlanViewModel(skillPlan,
                _skillSettingsMap[skillPlan.ConfigId],
                _container);
            _allSkillPlans.Add(skillPlanViewModel); //4
            _skillPlansMap[skillPlan.UniqueId] = skillPlanViewModel;
        }

        private void RemoveSkillPlanViewModel(SkillPlan skillPlan)
        {
            if (_skillPlansMap.TryGetValue(skillPlan.UniqueId, out var skillPlanViewModel))
            {
                _allSkillPlans.Remove(skillPlanViewModel);
                _skillPlansMap.Remove(skillPlan.UniqueId);
            }
        }

        /**
         * Пересчитываем параметры навыка в зависимости от уровня и эпичности
         */
        public void UpdateParameterSkillCard(SkillCard skillCard)
        {
            var settings = _skillSettingsMap[skillCard.ConfigId];
            
            skillCard.Parameters.Clear();
            foreach (var baseParameter in settings.BaseParameters)
            {
                skillCard.Parameters.Add(baseParameter.ParameterType, new SkillParameter(new SkillParameterData(baseParameter)));
            }
            

            //TODO Пересчет от базовых параметров 
            foreach (var keyValue in skillCard.Parameters)
            {
                //  Debug.Log(keyValue.Key);
                var skillParam = keyValue.Value;
                //Возвращаем базовое значение
                skillParam.Value.Value =
                    settings.BaseParameters.FirstOrDefault(p => p.ParameterType == skillParam.ParameterType)!.Value;

                //Обсчет эпичности
                var epicCardParam = settings.EpicCardParameters.Find(p => p.ParameterType == skillParam.ParameterType);
                if (epicCardParam != null)
                {
                    foreach (var epicParameter in epicCardParam.EpicParameters)
                    {
                        if (skillCard.EpicLevel.CurrentValue.Index() < epicParameter.Level.Index()) break;
                        skillParam.Value.Value *= (1 + epicParameter.Percent / 100);
                        if (epicParameter.Level == skillCard.EpicLevel.CurrentValue) break;
                    }
                }
                //Увеличиваем значения от уровня карты, если параметр изменчив
                var levelParam = settings.LevelCardParameters.Find(p => p.ParameterType == skillParam.ParameterType);
                if (levelParam != null)
                {
                    var rateEpic = Mathf.Pow(levelParam.PowEpic, skillCard.EpicLevel.Value.Index()); //Коэф.роста
                    skillParam.Value.Value += rateEpic * levelParam.BaseValue * (skillCard.Level.Value - 1);
                }
            }

 
        }


        public InfoUpgradedViewModel GetInfoUpgradedViewModel(string configId,
            TypeEpic epicLevel, int level)
        {
            var viewModel = new InfoUpgradedViewModel();
            var skillSettings = _skillSettingsMap[configId];
            var maxLevel = epicLevel.MaxLevel();
            viewModel.NameEnitity = skillSettings.TitleLid;
            viewModel.NameEpic = epicLevel.GetString();

            var skillCardData = new SkillCardData
            {
                EpicLevel = epicLevel,
                ConfigId = configId,
                Level = level,
                Parameters = new Dictionary<SkillParameterType, SkillParameterData>(),
                Defence = skillSettings.Defence,
            };
            foreach (var baseParameter in skillSettings.BaseParameters)
            {
                skillCardData.Parameters.Add(baseParameter.ParameterType, new SkillParameterData(baseParameter));
            }

            var skillCard = new SkillCard(skillCardData);
            skillCardData.EpicLevel = skillCardData.EpicLevel.Next();
            var skillCardAfter = new SkillCard(skillCardData);

            UpdateParameterSkillCard(skillCard);
            UpdateParameterSkillCard(skillCardAfter);

            viewModel.Parameters.Add(
                "МАКС.УРОВЕНЬ",
                new Vector2(skillCard.EpicLevel.Value.MaxLevel(), skillCardAfter.EpicLevel.Value.MaxLevel())
            );

            foreach (var valuePair in skillCard.Parameters)
            {
                if (!skillCardAfter.Parameters.TryGetValue(valuePair.Key, out var valueAfter))
                    throw new Exception("Ошибка");

                if (!Mathf.Approximately(valuePair.Value.Value.CurrentValue, valueAfter.Value.CurrentValue))
                    viewModel.Parameters.Add(valuePair.Key.GetString(),
                        new Vector2(valuePair.Value.Value.CurrentValue, valueAfter.Value.CurrentValue));
            }

            return viewModel;
        }
    }
}