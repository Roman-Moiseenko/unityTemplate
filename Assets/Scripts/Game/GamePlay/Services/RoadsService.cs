using System.Collections.Generic;
using Game.GamePlay.Commands.RoadCommand;
using Game.GamePlay.View.Roads;
using Game.State.Entities;
using Game.State.Maps.Roads;
using MVVM.CMD;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.Services
{
    public class RoadsService
    {
        private readonly IObservableCollection<RoadEntity> _way; //кешируем
        private readonly IObservableCollection<RoadEntity> _waySecond; //кешируем
        private readonly IObservableCollection<RoadEntity> _wayDisabled; //кешируем
        private readonly string _configIdDefault;
        private readonly ICommandProcessor _cmd;

        private readonly ObservableList<RoadViewModel> _allRoads = new();
        private readonly Dictionary<int, RoadViewModel> _roadsMap = new();
        

        public IObservableCollection<RoadViewModel> AllRoads =>
            _allRoads; //Интерфейс менять нельзя, возвращаем через динамический массив

        public RoadsService(
            IObservableCollection<RoadEntity> way,
            IObservableCollection<RoadEntity> waySecond,
            IObservableCollection<RoadEntity> wayDisabled,
            string configIdDefault,
            ICommandProcessor cmd
            )
        {
            _way = way;
            _waySecond = waySecond;
            _wayDisabled = wayDisabled;
            _configIdDefault = configIdDefault;
            _cmd = cmd;
            
            //Основной путь
            foreach (var roadEntity in way) CreateRoadViewModel(roadEntity);
            way.ObserveAdd().Subscribe(e => CreateRoadViewModel(e.Value));
            way.ObserveRemove().Subscribe(e => RemoveRoadViewModel(e.Value));
            
            
            //Второй путь
            foreach (var roadEntity in waySecond) CreateRoadViewModel(roadEntity);
            waySecond.ObserveAdd().Subscribe(e => CreateRoadViewModel(e.Value));
            waySecond.ObserveRemove().Subscribe(e => RemoveRoadViewModel(e.Value));
            
            //Бонусный путь (не доступный)
            foreach (var roadEntity in wayDisabled) CreateRoadViewModel(roadEntity);
            wayDisabled.ObserveAdd().Subscribe(e => CreateRoadViewModel(e.Value));
            wayDisabled.ObserveRemove().Subscribe(e => RemoveRoadViewModel(e.Value));
        }

        public bool PlaceRoad( Vector2Int position, Vector2Int pointEnter, Vector2Int pointExit, int rotate)
        {
            var command = new CommandPlaceRoad(_configIdDefault, position, pointEnter, pointExit, rotate);
            return _cmd.Process(command);
        }


        public bool MoveRoad(int roadId, Vector2Int position)
        {
            //var command = new CommandMoveTower(towerId, position);
            //return _cmd.Process(command);
            return false;
        }
        
        public bool RotateRoad(int roadId, Vector2Int position)
        {
            
            //var command = new CommandMoveTower(towerId, position);
            //return _cmd.Process(command);
            return false;
        }
        
        private void RemoveRoadViewModel(RoadEntity roadEntity)
        {
            
        }

        private void CreateRoadViewModel(RoadEntity roadEntity)
        {
            var roadViewModel = new RoadViewModel(roadEntity, this); //3
            _allRoads.Add(roadViewModel); //4
            _roadsMap[roadEntity.UniqueId] = roadViewModel;
        }


        private void CheckWay()
        {
            foreach (var road in _way)
            {
                
            }
        }
    }
}