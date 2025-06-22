using Game.State.Entities;
using Game.State.Maps.Roads;
using Game.State.Maps.Towers;
using Game.State.Root;
using MVVM.CMD;

namespace Game.GamePlay.Commands.RoadCommand
{
    public class CommandPlaceRoadHandler : ICommandHandler<CommandPlaceRoad>
    {
        private readonly GameplayStateProxy _gameplayState;

        public CommandPlaceRoadHandler(GameplayStateProxy gameplayState)
        {
            _gameplayState = gameplayState;
        }
        
        public bool Handle(CommandPlaceRoad command)
        {
            var entityId = _gameplayState.CreateEntityID(); //Получаем уникальный ID
            var newRoadEntity = new RoadEntityData() //Создаем сущность игрового объекта
            {
                UniqueId = entityId,
                Position = command.Position,
                PointEnter = command.PointEnter,
                PointExit = command.PointExit,
                ConfigId = command.RoadTypeId,
                Rotate = command.Rotate,
            };
            var newRoad = new RoadEntity(newRoadEntity);
            if (command.IsMainWay)
            {
                _gameplayState.Way.Add(newRoad);
            }
            else
            {
                _gameplayState.WaySecond.Add(newRoad);
            }
            
            return true;
        }
    }
}