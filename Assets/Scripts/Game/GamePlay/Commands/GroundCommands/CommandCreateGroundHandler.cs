using System;
using System.Linq;
using Game.State.Maps.Grounds;
using Game.State.Root;
using MVVM.CMD;
using UnityEngine;

namespace Game.GamePlay.Commands.GroundCommands
{
    public class CommandCreateGroundHandler : ICommandHandler<CommandCreateGround>
    {
        private readonly GameplayStateProxy _gameplayState;

        public CommandCreateGroundHandler(GameplayStateProxy gameplayState)
        {
            _gameplayState = gameplayState;
        }
        public bool Handle(CommandCreateGround command)
        {
            foreach (var ground in _gameplayState.Grounds)
            {
                if (ground.Position.CurrentValue == command.Position) return false;
            }
            
            
            var entityId = _gameplayState.CreateEntityID(); //Получаем уникальный ID
            var newgroundEntity = new GroundEntityData() //Создаем сущность игрового объекта
            {
                UniqueId = entityId,
                Position = command.Position,
                ConfigId = command.GroundType,
            };
            var newGround = new GroundEntity(newgroundEntity); //Оборачиваем его Прокси
            _gameplayState.Grounds.Add(newGround);//Добавляем в список объектов карты
            return true;
        }
    }
}