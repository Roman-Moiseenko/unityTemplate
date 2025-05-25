using System.Linq;
using Game.State.Entities.Buildings;
using ObservableCollections;
using R3;

namespace Game.State.Root
{
    public class GameStateProxy
    {
        private GameState _gameState;
        public ObservableList<BuildingEntityProxy> Buildings { get; } = new();

        public GameStateProxy(GameState gameState)
        {
            _gameState = gameState;
            gameState.Buildings.ForEach(
                buildingOriginal => Buildings.Add(new BuildingEntityProxy(buildingOriginal))
            );
            Buildings.ObserveAdd().Subscribe(e =>
            {
                var addedBuildingEntity = e.Value;
                gameState.Buildings.Add(addedBuildingEntity.Origin);
            });

            Buildings.ObserveRemove().Subscribe(e =>
            {
                var removedBuildingEntityProxy = e.Value;
                var removedBuildingEntity =
                    gameState.Buildings.FirstOrDefault(b => b.Id == removedBuildingEntityProxy.Id);
                gameState.Buildings.Remove(removedBuildingEntity);
            });
        }

        public int getEntityID()
        {
            return _gameState.GlobalEntityId++;
        }
    }
}