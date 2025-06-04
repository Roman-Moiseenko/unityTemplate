using System.Linq;
using Game.State.GameResources;
using Game.State.Inventory;
using Game.State.Maps;
using ObservableCollections;
using R3;
using UnityEngine;


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
        
        public GameplayState GameplayState;
        //public ReactiveProperty<GameplayStateData> GameplayStateData = new();
        public ObservableList<Map> Maps { get; } = new();
        public ObservableList<Inventory.Inventory> Inventory { get; } = new();

        public ObservableList<Resource> Resources { get; } = new();

        public GameStateProxy(GameState gameState)
        {
            _gameState = gameState;
            

            GameplayState = new GameplayState(gameState.GameplayStateData);
            
            GameplayState.GameSpeed.Subscribe(newSpeed =>
            {
                if (newSpeed == 0)
                {
                  //  gameState.PreviousGameSpeed;
                    Debug.Log("Игра на паузе");
                }
                else
                {
                    Debug.Log($"Скорость игры {newSpeed}");
                }
          //      gameState.StateData.GameSpeed = newSpeed;
                
            });

            InitMaps(gameState);
            InitResource(gameState);
            InitInventory(gameState);
            
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
           // Debug.Log(" ** " + JsonUtility.ToJson(gameState.Resources));
            gameState.Resources.ForEach(originResource => Resources.Add(new Resource(originResource)));
            Resources.ObserveAdd().Subscribe(e => gameState.Resources.Add(e.Value.Origin));

            Resources.ObserveRemove().Subscribe(e =>
            {
                var removedResourceData =
                    gameState.Resources.FirstOrDefault(b => b.ResourceType == e.Value.ResourceType);
                gameState.Resources.Remove(removedResourceData);
            });
        }

        private void InitInventory(GameState gameState)
        {
            gameState.Inventory.ForEach(originInventory => Inventory.Add(InventoryFactory.CreateInventory(originInventory)));
            Inventory.ObserveAdd().Subscribe(e => gameState.Inventory.Add(e.Value.Origin));
            
            Inventory.ObserveRemove().Subscribe(e =>
            {
                var removedInventoryData =
                    gameState.Inventory.FirstOrDefault(b => b.TypeItem == e.Value.TypeItem);
                gameState.Inventory.Remove(removedInventoryData);
            });
        }
        
        public int CreateEntityID()
        {
            return _gameState.CreateEntityID();
        }
    }
}