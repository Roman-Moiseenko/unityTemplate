using Game.GamePlay.Fsm;
using Game.GamePlay.Services;
using Game.GamePlay.View.Warriors;
using Game.State.Maps.Towers;
using Game.State.Maps.Warriors;
using Game.State.Root;
using ObservableCollections;
using R3;
using UnityEngine;


namespace Game.GamePlay.View.Towers
{
    public class TowerPlacementViewModel : TowerViewModel
    {
        public ObservableList<WarriorViewModel> Warriors = new();
        
        public float Range { get; set; }

        public float Health { get; set; }

        public float Damage { get; set; }

        public float Speed { get; set; }
        public TowerPlacementViewModel(TowerEntity towerEntity, GameplayStateProxy gameplayState,
            TowersService towersService, FsmTower fsmTower) : base(towerEntity, gameplayState, towersService, fsmTower)
        {
            
            UpdateParameterWarrior();
            
            for (var i = 1; i <= 3; i++)
            {
//                Debug.Log(" " + i);
                CreateWarriorViewModel();
                //    Debug.Log($"Воин {warriorEntityData.UniqueId} создан для башни {command.UniqueId}");
            }
        }

        private void UpdateParameterWarrior()
        {
            if (_towerEntity.Parameters.TryGetValue(TowerParameterType.Speed, out var towerSpeed))
                Speed = towerSpeed.Value;
            if (_towerEntity.Parameters.TryGetValue(TowerParameterType.Damage, out var towerDamage))
                Damage = towerDamage.Value;
            if (_towerEntity.Parameters.TryGetValue(TowerParameterType.Health, out var towerHealth))
                Health = towerHealth.Value;
            if (_towerEntity.Parameters.TryGetValue(TowerParameterType.Range, out var towerRange))
                Range = towerRange.Value;
        }

        /**
 * Башня призывающая воинов
 */
        public bool IsDeadAllWarriors()
        {
            return Warriors.Count == 0;
        }

        public void AddWarriorsTower()
        {
            CreateWarriorViewModel();
            CreateWarriorViewModel();
            CreateWarriorViewModel();
        }

        public void UpdateAndRestartWarriors()
        {
            //TODO Обновить солдат
        }

        private void CreateWarriorViewModel()
        {
            var warriorEntityData = new WarriorEntityData()
            {
                ParentId = _towerEntity.UniqueId,
                ConfigId = _towerEntity.ConfigId,
                Damage = Damage,
                Health = Health,
                MaxHealth = Health,
                Speed = Speed,
                Range = Range,
                    
                IsFly = _towerEntity.TypeEnemy == TowerTypeEnemy.Air,
                PlacementPosition = _towerEntity.Placement.CurrentValue, //Позиция, куда идт warrior первоначально
                StartPosition = _towerEntity.Position.CurrentValue, //Позиция башни, откуда идут warrior
                UniqueId = _gameplayState.CreateEntityID(),
                    
            };
            var warriorEntity = new WarriorEntity(warriorEntityData);
            var warriorViewModel = new WarriorViewModel(warriorEntity, _gameplayState);
            warriorViewModel.IsDead.Where(x => x)
                .Subscribe(_ => Warriors.Remove(warriorViewModel));
            Warriors.Add(warriorViewModel);
        }
    }
}