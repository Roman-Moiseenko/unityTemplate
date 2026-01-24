using System.Collections.Generic;
using Game.GamePlay.Services;
using Game.GamePlay.View.Warriors;
using Game.State.Maps.Towers;
using Game.State.Maps.Warriors;
using Game.State.Root;
using MVVM.CMD;
using UnityEngine;

namespace Game.GamePlay.Commands.WarriorCommands
{
    public class CommandCreateWarriorTowerHandler : ICommandHandler<CommandCreateWarriorTower>
    {
        private readonly GameplayStateProxy _gameplayState;
        private readonly Dictionary<string,Dictionary<TowerParameterType,TowerParameterData>> _towerParametersMap;

        public CommandCreateWarriorTowerHandler(GameplayStateProxy gameplayState, TowersService towersService)
        {
            _gameplayState = gameplayState;
            _towerParametersMap = towersService.TowerParametersMap;
        }
        public bool Handle(CommandCreateWarriorTower command)
        {
            var parameters = _towerParametersMap[command.ConfigId];

            parameters.TryGetValue(TowerParameterType.Damage, out var damage);
            parameters.TryGetValue(TowerParameterType.Speed, out var speed);
            parameters.TryGetValue(TowerParameterType.Health, out var health);
            parameters.TryGetValue(TowerParameterType.Range, out var range);
            
            for (var i = 1; i <= 3; i++)
            {
                Debug.Log(" " + i);
                var warriorEntityData = new WarriorEntityData()
                {
                    ParentId = command.UniqueId,
                    ConfigId = command.ConfigId,
                    Damage = damage.Value,
                    Health = health.Value,
                    MaxHealth = health.Value,
                    Speed = speed.Value,
                    IsFly = command.TypeEnemy == TowerTypeEnemy.Air,
                    PlacementPosition = command.Placement, //Позиция, куда идт warrior первоначально
                    StartPosition = command.Position, //Позиция башни, откуда идут warrior
                    UniqueId = _gameplayState.CreateEntityID(),
                    Range = range.Value
                };
                var warriorEntity = new WarriorEntity(warriorEntityData);
                _gameplayState.Warriors.Add(warriorEntity);
                Debug.Log($"Воин {warriorEntityData.UniqueId} создан для башни {command.UniqueId}");
            }
            
            return false;
        }
    }
}