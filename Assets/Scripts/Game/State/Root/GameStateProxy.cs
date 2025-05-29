using System.Linq;
using Game.State.Entities.Buildings;
using Game.State.GameResources;
using Game.State.Maps;
using ObservableCollections;
using R3;

namespace Game.State.Root
{
    /**
     * Прокси обертка всего стостояния игры (список ресурсов игрока, все данные по игре)
     * Подписка на события Добавления и удаления из списков объектов / изменения
     */
    public class GameStateProxy
    {
        private readonly GameState _gameState;
        public ReactiveProperty<int> CurrentMapId = new();
        public ObservableList<Map> Maps { get; } = new();

        public ObservableList<Resource> Resources { get; } = new();

        public GameStateProxy(GameState gameState)
        {
            _gameState = gameState;

            InitMaps(gameState);

            InitResource(gameState);

            CurrentMapId.Subscribe(newValue => { gameState.CurrentMapId = newValue; });
        }

        private void InitMaps(GameState gameState)
        {
            gameState.Maps.ForEach(
                mapOriginal => Maps.Add(new Map(mapOriginal))
            );
            Maps.ObserveAdd().Subscribe(e => gameState.Maps.Add(e.Value.Origin));

            Maps.ObserveRemove().Subscribe(e =>
            {
                var removedMapState = gameState.Maps.FirstOrDefault(b => b.Id == e.Value.Id);
                gameState.Maps.Remove(removedMapState);
            });
        }

        private void InitResource(GameState gameState)
        {
            gameState.Resources.ForEach(originResource => Resources.Add(new Resource(originResource)));
            Resources.ObserveAdd().Subscribe(e => gameState.Resources.Add(e.Value.Origin));

            Resources.ObserveRemove().Subscribe(e =>
            {
                var removedResourceData =
                    gameState.Resources.FirstOrDefault(b => b.ResourceType == e.Value.ResourceType);
                gameState.Resources.Remove(removedResourceData);
            });
        }

        public int CreateEntityID()
        {
            return _gameState.CreateEntityID();
        }
    }
}