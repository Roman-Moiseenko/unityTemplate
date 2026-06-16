using System;
using System.Collections.Generic;
using System.Linq;
using DI;
using Game.MainMenu.View.ScreenInventory.HeroCards;
using Game.Settings.Gameplay.Entities.Heroes;
using Game.State.Inventory.Common;
using Game.State.Inventory.Deck;
using Game.State.Inventory.HeroCards;
using MVVM.CMD;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.MainMenu.Services
{
    public class HeroCardService : IDisposable
    {
        public IObservableCollection<HeroCardViewModel> AllHeroCards => _allHeroCards;

        private readonly ObservableList<HeroCardViewModel> _allHeroCards = new();
        private readonly Dictionary<int, HeroCardViewModel> _heroCardsMap = new();
        private readonly Dictionary<string, HeroSettings> _heroSettingsMap = new();
        private readonly DeckCard _currentDeck;
        
        private readonly InventoryRoot _inventoryRoot;
        private readonly IObservableCollection<InventoryItem> _items;
        private readonly ICommandProcessor _cmd;
        private readonly DIContainer _container;
        private DisposableBag _disposables;

        public HeroCardService(
            InventoryRoot inventoryRoot,
            HeroesSettings heroesSettings,
            ICommandProcessor cmd,
            DIContainer container
        )
        {
            _inventoryRoot = inventoryRoot;
            _items = inventoryRoot.Items;
            _cmd = cmd;
            _container = container;
            _currentDeck = _inventoryRoot.GetCurrentDeckCard();

            //Кешируем настройки для быстрого поиска
            foreach (var heroSettings in heroesSettings.AllHeroes)
            {
                _heroSettingsMap[heroSettings.ConfigId] = heroSettings;
            }

            //Проходим список всех карточек героев и обновить базовые параметры по их Level и Rank
            foreach (var item in _items)
            {
                if (item is HeroCard heroCard)
                {
                    heroCard.Rank.Skip(1)
                        .Subscribe(e => UpdateParameterHeroCard(heroCard))
                        .AddTo(ref _disposables);
                    heroCard.Level
                        .Subscribe(e => UpdateParameterHeroCard(heroCard))
                        .AddTo(ref _disposables);
                    UpdateParameterHeroCard(heroCard);
                    CreateHeroCardViewModel(heroCard);
                }
            }

            var heroView = _allHeroCards
                .FirstOrDefault(t => t.ConfigId == _currentDeck.HeroConfigId.CurrentValue);
            heroView?.IsDeck.OnNext(true);
        }

        private void UpdateParameterHeroCard(HeroCard heroCard)
        {
            var settings = _heroSettingsMap[heroCard.ConfigId];
            heroCard.Parameters.Clear();
            foreach (var baseParameter in settings.BaseParameters)
            {
                heroCard.AddParameter(baseParameter);
            }

            //Пересчитываем базовые параметры
            foreach (var (typeParam, heroParam) in heroCard.Parameters)
            {
                //Обсчет ранга
                var rankCardParam = settings.RankCardParameters.Find(p => p.ParameterType == typeParam);
                if (rankCardParam != null) //Находим параметр в списке, если есть, то увеличиваем
                {
                    foreach (var rankValue in rankCardParam.Values) //Проходим все Values из настроек
                    {
                        if (rankValue.Rank <= heroCard.Rank.CurrentValue) //Если ранг настройки <= ранга героя
                            heroParam.Value.Value += rankValue.Value;
                    }
                }

                //Обсчет уровня
                var levelCardParam = settings.LevelCardParameters.Find(p => p.ParameterType == typeParam);
                if (levelCardParam != null)
                {
                    var rateLevel = Mathf.Pow(levelCardParam.PowEpic, heroCard.Rank.CurrentValue);
                    heroParam.Value.Value += rateLevel * levelCardParam.BaseValue * (heroCard.Level.Value - 1);
                }
            }
        }
        
        private void CreateHeroCardViewModel(HeroCard heroCard)
        {
            var heroCardViewModel = new HeroCardViewModel(
                heroCard,
                _heroSettingsMap[heroCard.ConfigId],
                _container
            );

            _allHeroCards.Add(heroCardViewModel);
            _heroCardsMap[heroCard.UniqueId] = heroCardViewModel;
        }


        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}