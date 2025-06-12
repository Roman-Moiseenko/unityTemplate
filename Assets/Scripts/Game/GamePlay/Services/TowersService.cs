
using System.Collections.Generic;
using Game.GamePlay.Commands.TowerCommand;
using Game.GamePlay.View.Towers;
using Game.Settings.Gameplay.Entities.Tower;
using Game.State.Entities;
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
        private readonly IObservableCollection<Entity> _entities; //кешируем
        private readonly ICommandProcessor _cmd;

        private readonly ObservableList<TowerViewModel> _allTowers = new();
        private readonly Dictionary<int, TowerViewModel> _towersMap = new();
        private readonly Dictionary<string, TowerSettings> _towerSettingsMap = new();
        public IObservableCollection<TowerViewModel> AllTowers => _allTowers; //Интерфейс менять нельзя, возвращаем через динамический массив
        public readonly ObservableDictionary<string, int> Levels = new(); //Уровни башен по типам ConfigId
        
        
        /**
         * При загрузке создаем все view-модели из реактивного списка всех строений 
         * Подписываемся на событие добавления в массив Proxy сущностей
        */
         
        public TowersService(
            IObservableCollection<Entity> entities,
            TowersSettings towersSettings, 
            ICommandProcessor cmd
            )
        {
            _entities = entities;
            _cmd = cmd;

            //Кешируем настройки зданий / обектов
            foreach (var towerSettings in towersSettings.AllTowers)
            {
                _towerSettingsMap[towerSettings.ConfigId] = towerSettings;
                Levels[towerSettings.ConfigId] = 1;
            }
            
            foreach (var entity in entities)
            {
                if (entity is TowerEntity towerEntity) CreateTowerViewModel(towerEntity);
            }
            
            //Подписка на добавление новых view-моделей текущего класса
            entities.ObserveAdd().Subscribe(e =>
            {
                if (e.Value is TowerEntity towerEntity)
                {
                    towerEntity.Level.Value = Levels[towerEntity.ConfigId]; //Устанавливаем уровень апгрейда
                    CreateTowerViewModel(towerEntity);
                }
            });
            //Если у сущности изменился уровень, меняем его и во вью-модели
            entities.ObserveRemove().Subscribe(e =>
            {
                if (e.Value is TowerEntity towerEntity) RemoveTowerViewModel(towerEntity);
            });
            
            Levels.ObserveChanged().Subscribe(x =>
            {
               //  Debug.Log(" Меняем уровнии для " + x.NewItem.Key + " на " + x.NewItem.Value);

                foreach (var entity in _entities)
                {
                    if (entity is TowerEntity towerEntity && towerEntity.ConfigId == x.NewItem.Key)
                    {
                        //Debug.Log("Нашли towerEntity " + towerEntity.ConfigId + " ЛЕВЕЛ = " + towerEntity.Level.Value + " с Id" + towerEntity.UniqueId );

                        RemoveTowerViewModel(towerEntity);//TODO Удаляем все модели viewModel.ConfigId == x.NewItem.Key
                        
                        CreateTowerViewModel(towerEntity);//TODO Создаем модели Заново
                    }
                }

                // Debug.Log(" Меняем уровнии для " + x.NewItem.Key + " на " + x.NewItem.Value);
                /*_allTowers.ForEach(viewModel =>
                {
                    if (viewModel.ConfigId == x.NewItem.Key) viewModel.Level.Value = x.NewItem.Value;
                }); */
                
                
                
            });
            

        }


        public bool PlaceTower(string towerTypeId, Vector2Int position)
        {
            var command = new CommandPlaceTower(towerTypeId, position);
            return _cmd.Process(command);
        }

        public bool MoveTower(int towerId, Vector2Int position)
        {
            return false;
        }

        public bool DeleteTower(int towerId)
        {
            return false;
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
            var towerViewModel = new TowerViewModel(towerEntity, _towerSettingsMap[towerEntity.ConfigId], this); //3
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

        public bool ApplyBust(string configIdTower, string configIdBust)
        {
            //Повышаем уровень башен
            //Нужно для кеширования при строительстве новой башни и замены модели
            var command = new CommandTowerBust(configIdTower, configIdBust);
            if (_cmd.Process(command))
            {
                Levels[configIdTower] += 1;
                return true;
            }

            return false;

        }

        public void ReplaceTower(int cardUniqueId, object cardUniqueId2)
        {
            //TODO Меняем местами две башни
        }
    }
}
