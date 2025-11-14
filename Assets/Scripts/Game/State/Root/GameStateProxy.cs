using System;
using Game.State.GameStates;
using Game.State.Inventory;
using Game.State.Inventory.Chests;
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
        public readonly GameState Origin;
        public const int MaxChest = 4;
        //private readonly GameState _gameState;
        public ReactiveProperty<int> CurrentMapId = new();
        public ReactiveProperty<int> GameSpeed;
        public ReactiveProperty<int> HardCurrency;
        public ReactiveProperty<long> SoftCurrency;

        public InventoryRoot Inventory { get; set; }
        public MapStates MapStates { get; set; }
        public ContainerChests ContainerChests { get; set; }
        
        //public ObservableDictionary<int, Chest> ListChests;


        public GameStateProxy(GameState gameState)
        {
            Origin = gameState;
            //_gameState = gameState;

            GameSpeed = new ReactiveProperty<int>(gameState.GameSpeed);
            GameSpeed.Subscribe(newValue =>
            {
                gameState.GameSpeed = newValue;
                UpdateDateVersion();
                Debug.Log($"Сохраняем скорость игры в GameState = {newValue}");
            });
            SoftCurrency = new ReactiveProperty<long>(gameState.SoftCurrency);
            SoftCurrency.Subscribe(newValue =>
            {
                gameState.SoftCurrency = newValue;
                UpdateDateVersion();
            });

            HardCurrency = new ReactiveProperty<int>(gameState.HardCurrency);
            HardCurrency.Subscribe(newValue =>
            {
                gameState.HardCurrency = newValue;
                UpdateDateVersion();
            });

            Inventory = new InventoryRoot(gameState.Inventory);
            ContainerChests = new ContainerChests(gameState.ContainerChests);
            CurrentMapId.Subscribe(newValue =>
            {
                gameState.CurrentMapId = newValue;
                UpdateDateVersion();
            });

            MapStates = new MapStates(gameState.MapStatesData);

            MapStates.UpdateData.Subscribe(_ => UpdateDateVersion());
            Inventory.UpdateData.Subscribe(_ => UpdateDateVersion());
            ContainerChests.UpdateData.Subscribe(_ => UpdateDateVersion());

        }

        private void InitResource(GameState gameState)
        {
            // Debug.Log(" ** " + JsonUtility.ToJson(gameState.Resources));
            /*   gameState.Resources.ForEach(originResource => Resources.Add(new Resource(originResource)));
               Resources.ObserveAdd().Subscribe(e => gameState.Resources.Add(e.Value.Origin));

               Resources.ObserveRemove().Subscribe(e =>
               {
                   var removedResourceData =
                       gameState.Resources.FirstOrDefault(b => b.ResourceType == e.Value.ResourceType);
                   gameState.Resources.Remove(removedResourceData);
               });*/
        }

     /*   private void InitChests(GameState gameState)
        {
            ListChests = new ObservableDictionary<int, Chest>();
            foreach (var (cell, chestData) in gameState.Chests)
            {
                ListChests.Add(cell, new Chest(chestData));
            }

            ListChests.ObserveAdd().Subscribe(e =>
            {
                var cell = e.Value.Key;
                var chest = e.Value.Value;
                gameState.Chests.Add(cell, chest.Origin);
            });
            ListChests.ObserveRemove().Subscribe(e => { gameState.Chests.Remove(e.Value.Key); });
        }
*/

        public int CreateInventoryID()
        {
            return Origin.CreateInventoryID();
        }

        public bool PaidHardCurrency(int value)
        {
            //TODO Возможно вызвать событие @Нехватка денег@
            if (HardCurrency.CurrentValue < value) return false;
            HardCurrency.Value -= value;
            //TODO Сохранить данные!!!!!
            return true;
        }

        private void UpdateDateVersion()
        {
            Origin.DateVersion = DateTime.Now;
        }
        
    }
}