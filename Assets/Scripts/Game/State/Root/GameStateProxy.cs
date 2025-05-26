using System.Linq;
using Game.State.Entities.Buildings;
using Game.State.Maps;
using ObservableCollections;
using R3;

namespace Game.State.Root
{
    public class GameStateProxy
    {
        private readonly GameState _gameState;
        public ReactiveProperty<int> CurrentMapId = new();
        public ObservableList<Map> Maps { get; } = new();

        public GameStateProxy(GameState gameState)
        {
            _gameState = gameState;
            gameState.Maps.ForEach(
                mapOriginal => Maps.Add(new Map(mapOriginal))
            );
            Maps.ObserveAdd().Subscribe(e =>
            {
                var addedMap = e.Value;
                gameState.Maps.Add(addedMap.Origin);
            });

            Maps.ObserveRemove().Subscribe(e =>
            {
                var removedMap = e.Value;
                var removedMapState =
                    gameState.Maps.FirstOrDefault(b => b.Id == removedMap.Id);
                gameState.Maps.Remove(removedMapState);
            });
            CurrentMapId.Subscribe(newValue => { gameState.CurrentMapId = newValue; });

        }

        public int CreateEntityID()
        {
            return _gameState.CreateEntityID();
        }
    }
}