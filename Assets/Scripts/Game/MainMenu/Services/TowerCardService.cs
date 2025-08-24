using System;
using System.Collections.Generic;
using System.Linq;
using Game.MainMenu.View.ScreenInventory.TowerCards;
using Game.Settings.Gameplay.Entities.Tower;
using Game.State.Inventory;
using Game.State.Inventory.TowerCards;
using Game.State.Maps.Towers;
using MVVM.CMD;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.MainMenu.Services
{
    public class TowerCardService
    {
        private readonly IObservableCollection<InventoryItem> _items; //кешируем
        private readonly ICommandProcessor _cmd;

        private readonly ObservableList<TowerCardViewModel> _allTowerCards = new();
        private readonly Dictionary<int, TowerCardViewModel> _towerCardsMap = new();
        private readonly Dictionary<string, TowerSettings> _towerSettingsMap = new();

        public IObservableCollection<TowerCardViewModel> AllTowerCards =>
            _allTowerCards; //Интерфейс менять нельзя, возвращаем через динамический массив

        public TowerCardService(
            InventoryRoot inventoryRoot,
            TowersSettings towersSettings,
            ICommandProcessor cmd
        )
        {
            _items = inventoryRoot.Items;
            _cmd = cmd;

            //Кешируем настройки зданий / обектов
            foreach (var towerSettings in towersSettings.AllTowers)
            {
                _towerSettingsMap[towerSettings.ConfigId] = towerSettings;
            }

            foreach (var item in _items)
            {
                if (item is TowerCard towerCard)
                {
                    towerCard.EpicLevel.Subscribe(e => UpdateParameterTowerCard(towerCard));
                    towerCard.Level.Subscribe(e => UpdateParameterTowerCard(towerCard));
                    CreateTowerCardViewModel(towerCard);
                }
            }

            _items.ObserveAdd().Subscribe(e =>
            {
                if (e.Value is TowerCard towerCard)
                {
                    towerCard.EpicLevel.Subscribe(_ => UpdateParameterTowerCard(towerCard));
                    towerCard.Level.Subscribe(_ => UpdateParameterTowerCard(towerCard));
                    CreateTowerCardViewModel(towerCard); //Создаем View Model
                }
            });
            //Если у сущности изменился уровень, меняем его и во вью-модели
            _items.ObserveRemove().Subscribe(e =>
            {
                if (e.Value is TowerCard towerCard) RemoveTowerCardViewModel(towerCard);
            });
        }


        private void CreateTowerCardViewModel(TowerCard towerCard)
        {
            var towerCardViewModel = new TowerCardViewModel(towerCard, _towerSettingsMap[towerCard.ConfigId], this); //3

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

        /**
         * Пересчитываем параметры башни в зависимости от уровня и эпичности
         */
        private void UpdateParameterTowerCard(TowerCard towerCard)
        {
            var epic = towerCard.EpicLevel.Value;
            var level = towerCard.Level.Value;
            var settings = _towerSettingsMap[towerCard.ConfigId];
            
            foreach (var keyValue in towerCard.Parameters)
            {
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
            }

            //Debug.Log("UpdateParameterTowerCard для " + towerCard.UniqueId + $" ({towerCard.ConfigId})");
        }
    }
}