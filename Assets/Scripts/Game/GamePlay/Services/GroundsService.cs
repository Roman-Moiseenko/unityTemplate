using System.Collections.Generic;
using Game.GamePlay.Commands.GroundCommands;
using Game.GamePlay.View.Grounds;
using Game.Settings.Gameplay.Grounds;
using Game.State.Entities;
using Game.State.Maps.Grounds;
using MVVM.CMD;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.Services
{
    public class GroundsService
    {
        private readonly string _configIdDefault;
        private readonly ICommandProcessor _cmd;

        private readonly ObservableList<GroundViewModel> _allGrounds = new();
        private readonly Dictionary<int, GroundViewModel> _groundsMap = new();
   //     private readonly Dictionary<string, GroundSettings> _groundSettingsMap = new();

        public IObservableCollection<GroundViewModel> AllGrounds =>
            _allGrounds; //Интерфейс менять нельзя, возвращаем через динамический массив

        public GroundsService(
            IObservableCollection<GroundEntity> grounds,
            string configIdDefault,
           // GroundSettings groundSettings,
            ICommandProcessor cmd
            )
        {
            _configIdDefault = configIdDefault;
            _cmd = cmd;
            
            
            foreach (var ground in grounds)
            {
                CreateGroundViewModel(ground);
            }
            
            //Подписка на добавление новых view-моделей текущего класса
            grounds.ObserveAdd().Subscribe(e =>
            {
                CreateGroundViewModel(e.Value);
            });
            // и на удаление
            grounds.ObserveRemove().Subscribe(e =>
            {
                RemoveGroundViewModel(e.Value);
            });
            
        }
        
        public bool PlaceGround(Vector2Int position)
        {
            var command = new CommandCreateGround(_configIdDefault, position);
            return _cmd.Process(command);
        }
        

        public bool DeleteGround(Vector2Int position)
        {
            var command = new CommandRemoveGround(position);
            return _cmd.Process(command);
        }


        /**
         * 1. По параметрам создается сущность GroundEntity
         * 2. Оборачивается Proxy для навешивания реактивности и событий
         * 3. На основе Proxy сущности создается view-модель
         * 4. Модель добавляем в словарь всех моделей данного класса
         * 5. Кешируем Id и view-модели
         */
        
        private void CreateGroundViewModel(GroundEntity groundEntity)
        {
            var groundViewModel = new GroundViewModel(groundEntity, this); //3
            _allGrounds.Add(groundViewModel); //4
            _groundsMap[groundEntity.UniqueId] = groundViewModel;
        }

        /**
         * Удаляем объект из списка моделей и из кеша
         */

        private void RemoveGroundViewModel(GroundEntity groundEntity)
        {
            if (_groundsMap.TryGetValue(groundEntity.UniqueId, out var groundViewModel))
            {
                _allGrounds.Remove(groundViewModel);
                _groundsMap.Remove(groundEntity.UniqueId);
            }
        }
    }
}