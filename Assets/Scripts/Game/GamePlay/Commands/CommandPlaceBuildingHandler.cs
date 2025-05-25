using Game.State.CMD;
using Game.State.Entities.Buildings;
using Game.State.Root;

namespace Game.GamePlay.Commands
{
    public class CommandPlaceBuildingHandler : ICommandHandler<CommandPlaceBuilding>
    {
        private readonly GameStateProxy _gameState;

        public CommandPlaceBuildingHandler(GameStateProxy gameState)
        {
            _gameState = gameState;
        }
        
        public bool Handle(CommandPlaceBuilding command)
        {
            var entityId = _gameState.getEntityID(); //Получаем уникальный ID
            var newBuildingEntity = new BuildingEntity //Создаем сущность игрового объекта
            {
                Id = entityId,
                Position = command.Position,
                TypeId = command.BuildingTypeId
            };
            var newBuildingEntityProxy = new BuildingEntityProxy(newBuildingEntity); //Оборачиваем его Прокси
            _gameState.Buildings.Add(newBuildingEntityProxy); //Добавляем в список объектов игрового мира
            return true;
        }
    }
}