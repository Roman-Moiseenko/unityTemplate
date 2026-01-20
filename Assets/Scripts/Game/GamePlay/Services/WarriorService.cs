using Game.State.Entities;
using Game.State.Maps.Towers;
using Game.State.Maps.Warriors;
using Game.State.Root;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.Services
{
    public class WarriorService
    {
        private ObservableList<WarriorEntity> _allWarriors = new();

        private readonly TowersService _towersService;
        private readonly GameplayStateProxy _gameplayState;

        //private readonly Dictionary<string, Dictionary<TowerParameterType, TowerParameterData>> TowerParametersMap = new();
        public WarriorService(TowersService towersService, GameplayStateProxy gameplayState)
        {
            _towersService = towersService;
            _gameplayState = gameplayState;
        }

        public void AddWarriorsTower(TowerEntity towerEntity)
        {
            var placement = towerEntity.Placement;
            var parameters = _towersService.TowerParametersMap[towerEntity.ConfigId];

            parameters.TryGetValue(TowerParameterType.Damage, out var damage);
            parameters.TryGetValue(TowerParameterType.Speed, out var speed);
            parameters.TryGetValue(TowerParameterType.Health, out var health);
            parameters.TryGetValue(TowerParameterType.Range, out var range);

            var isFly = towerEntity.TypeEnemy == TowerTypeEnemy.Air;
            var position = new Vector3(towerEntity.Position.CurrentValue.x, isFly ? 1 : 0,
                towerEntity.Position.CurrentValue.y);
            const float delta = 0.2f;
            
            for (var i = -1; i < 2; i++)
            {
                var startPosition = new Vector3(
                    towerEntity.Placement.CurrentValue.x + delta * i,
                    isFly ? 1 : 0,
                    towerEntity.Placement.CurrentValue.y + delta * i
                );

                var warriorEntityData = new WarriorEntityData()
                {
                    ParentId = towerEntity.UniqueId,
                    ConfigId = towerEntity.ConfigId,
                    Damage = damage.Value,
                    Health = health.Value,
                    MaxHealth = health.Value,
                    Speed = speed.Value,
                    IsFly = isFly,
                    Position = position,
                    StartPosition = startPosition,
                    UniqueId = _gameplayState.CreateEntityID(),
                    Range = range.Value
                };
                _allWarriors.Add(new WarriorEntity(warriorEntityData));
            }
        }

        public bool AllWarriorsIDead(int towerId)
        {
            foreach (var warriorEntity in _allWarriors)
            {
                if (warriorEntity.ParentId == towerId) return false;
            }

            return true;
        }
    }
}