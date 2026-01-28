using System;
using System.Linq;
using Game.GamePlay.Commands.WarriorCommands;
using Game.GamePlay.View.Warriors;
using Game.State.Entities;
using Game.State.Maps.Towers;
using Game.State.Maps.Warriors;
using Game.State.Root;
using MVVM.CMD;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.Services
{
    public class WarriorService
    {
        private readonly ObservableList<WarriorEntity> _allWarriorEntities = new();
        private readonly ObservableList<WarriorViewModel> _allWarriors = new();
        public IObservableCollection<WarriorViewModel> AllWarriors => _allWarriors;

        private readonly TowersService _towersService;
        private readonly GameplayStateProxy _gameplayState;
        private readonly ICommandProcessor _cmd;

        //private readonly Dictionary<string, Dictionary<TowerParameterType, TowerParameterData>> TowerParametersMap = new();
        public WarriorService(GameplayStateProxy gameplayState, ICommandProcessor cmd)
        {
            _gameplayState = gameplayState;
            _cmd = cmd;
            foreach (var warriorEntity in gameplayState.Warriors)
            {
                Debug.Log(" " + warriorEntity.UniqueId + " " + warriorEntity.ParentId);
                CreateWarriorViewModel(warriorEntity);
            }
            
            gameplayState.Warriors.ObserveAdd().Subscribe(e =>
            {
                var warriorEntity = e.Value;
                CreateWarriorViewModel(warriorEntity);
                warriorEntity.IsDead.Skip(1).Where(x => x).Subscribe(
                    _ => RemoveWarrior(warriorEntity));
            });
            gameplayState.Warriors.ObserveRemove().Subscribe();
            
            gameplayState.Warriors.ObserveRemove().Subscribe(e =>
            {
                var warriorEntity = e.Value;
//                Debug.Log(" Удален warriorEntity " + warriorEntity.UniqueId);
                RemoveWarriorViewModel(warriorEntity);
            });
            
        }

        private void RemoveWarriorViewModel(WarriorEntity warriorEntity)
        {
            foreach (var warriorViewModel in _allWarriors.ToList())
            {
                if (warriorViewModel.UniqueId == warriorEntity.UniqueId)
                {
                    _allWarriors.Remove(warriorViewModel);
                }
            }
        }

        private void CreateWarriorViewModel(WarriorEntity warriorEntity)
        {
            var warriorViewModel = new WarriorViewModel(warriorEntity, _gameplayState);
            _allWarriors.Add(warriorViewModel);
        }

        public void AddWarriorsTower(TowerEntity towerEntity)
        {
            var command = new CommandCreateWarriorTower
            {
                UniqueId = towerEntity.UniqueId,
                ConfigId = towerEntity.ConfigId,
                TypeEnemy = towerEntity.TypeEnemy,
                Position = towerEntity.Position.CurrentValue,
                Placement = towerEntity.Placement.CurrentValue,
            };
            _cmd.Process(command);
        }

        /**
         * Все воины, выпущенные башней towerId мертвы
         */
        public bool IsDeadAllWarriors(int uniqueId)
        {
            foreach (var warriorViewModel in _allWarriors)
            {
                if (warriorViewModel.ParentId == uniqueId) return false;
            }

            return true;
        }

        public void RemoveWarrior(WarriorEntity warriorEntity)
        {
            var command = new CommandRemoveWarriorTower
            {
                UniqueId = warriorEntity.UniqueId,
            };
            _cmd.Process(command);
        }
    }
}