using System.Linq;
using Game.State.GameResources;
using Game.State.Inventory;
using Game.State.Inventory.Deck;
using Game.State.Inventory.TowerCards;
using Game.State.Maps;
using Newtonsoft.Json;
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
        public ReactiveProperty<int> GameSpeed;
        public ReactiveProperty<int> HardCurrency; 
        public ObservableList<Resource> Resources { get; } = new();
     //   public ObservableList<Map> Maps { get; } = new();
        public ObservableList<InventoryItem> InventoryItems { get; } = new();


    //    public ObservableList<TowerCard> TowerCards { get; set; }
       //// public ObservableDictionary<string, TowerPlanData> TowerPlans { get; set; } = new();
        
        public ObservableDictionary<int, DeckCard> DeckCards { get; set; } //Колоды карт

        public ReactiveProperty<int> BattleDeck;

        public GameStateProxy(GameState gameState)
        {
            _gameState = gameState;

            GameSpeed = new ReactiveProperty<int>(gameState.GameSpeed);
            GameSpeed.Subscribe(newValue =>
            {
                gameState.GameSpeed = newValue;
                Debug.Log($"Сохраняем скорость игры в GameState = {newValue}");
            });

            HardCurrency = new ReactiveProperty<int>(gameState.HardCurrency);
            HardCurrency.Subscribe(newValue => gameState.HardCurrency = newValue);
            
            
       //     InitMaps(gameState);
            InitResource(gameState);
            
            InitInventory(gameState);
            
            CurrentMapId.Subscribe(newValue => { gameState.CurrentMapId = newValue; });
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
            BattleDeck = new ReactiveProperty<int>(gameState.BattleDeck);
            BattleDeck.Subscribe(newValue => gameState.BattleDeck = newValue);
            DeckCards = new ObservableDictionary<int, DeckCard>();
            
            foreach (var keyValue in gameState.DeckCards)
            {
                DeckCards.Add(keyValue.Key, new DeckCard(keyValue.Value));
            }

            DeckCards.ObserveAdd().Subscribe(e =>
            {
                gameState.DeckCards.Add(e.Value.Key, e.Value.Value.Origin);
            });
            DeckCards.ObserveChanged().Subscribe(e =>
            {
                //TODO ?
            });
/*
            TowerCards = new ObservableList<TowerCard>();
            foreach (var towerCardData in gameState.TowerCards)
            {
                TowerCards.Add(new TowerCard(towerCardData));
            }
            TowerCards.ObserveAdd().Subscribe(e => gameState.TowerCards.Add((TowerCardData)e.Value.Origin));
            TowerCards.ObserveRemove().Subscribe(e =>
            {
                var removedTowerCard = gameState.TowerCards.FirstOrDefault(c => c.UniqueId == e.Value.UniqueId);
                gameState.TowerCards.Remove(removedTowerCard);
            });
            */
            gameState.InventoryItems.ForEach(originInventory => InventoryItems.Add(InventoryFactory.CreateInventory(originInventory)));
            InventoryItems.ObserveAdd().Subscribe(e => gameState.InventoryItems.Add(e.Value.Origin));
            
            InventoryItems.ObserveRemove().Subscribe(e =>
            {
                var removedInventoryData =
                    gameState.InventoryItems.FirstOrDefault(b => b.UniqueId == e.Value.UniqueId);
                gameState.InventoryItems.Remove(removedInventoryData);
            });
        }
        
        public int CreateInventoryID()
        {
            return _gameState.CreateInventoryID();
        }

        public bool PaidHardCurrency(int value)
        {
            //TODO Возможно вызвать событие @Нехватка денег@
            if (HardCurrency.CurrentValue < value) return false;
            HardCurrency.Value -= value;
            //TODO Сохранить данные!!!!!
            return true;
        }
    }
}