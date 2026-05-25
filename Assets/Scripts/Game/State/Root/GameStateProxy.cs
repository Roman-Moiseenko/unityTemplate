using System;
using Game.State.GameStates;
using Game.State.Inventory;
using Game.State.Inventory.Chests;
using Game.State.Inventory.Common;
using ObservableCollections;
using R3;
using UnityEngine;


namespace Game.State.Root
{
    /**
     * Прокси обертка всего стостояния игры (список ресурсов игрока, все данные по игре)
     * Подписка на события Добавления и удаления из списков объектов / изменения
     */
    public class GameStateProxy : IDisposable
    {
        public readonly GameState Origin;
        public const int MaxChest = 4;
        //private readonly GameState _gameState;
        public ReactiveProperty<int> CurrentMapId = new();
        public ReactiveProperty<float> GameSpeed;
        public ReactiveProperty<long> HardCurrency;
        public ReactiveProperty<long> SoftCurrency;

        public InventoryRoot Inventory { get; set; }
        public MapStates MapStates { get; set; }
        public ContainerChests ContainerChests { get; set; }
        
        //public ObservableDictionary<int, Chest> ListChests;
        private DisposableBag _disposables = new();

        public GameStateProxy(GameState gameState)
        {
            Origin = gameState;
            //_gameState = gameState;
            
            GameSpeed = new ReactiveProperty<float>(gameState.GameSpeed);
            GameSpeed.Subscribe(newValue =>
            {
                gameState.GameSpeed = newValue;
                UpdateDateVersion();
               // Debug.Log($"Сохраняем скорость игры в GameState = {newValue}");
            }).AddTo(ref _disposables);
            SoftCurrency = new ReactiveProperty<long>(gameState.SoftCurrency);
            SoftCurrency.Subscribe(newValue =>
            {
                gameState.SoftCurrency = newValue;
                UpdateDateVersion();
            }).AddTo(ref _disposables);

            HardCurrency = new ReactiveProperty<long>(gameState.HardCurrency);
            HardCurrency.Subscribe(newValue =>
            {
                gameState.HardCurrency = newValue;
                UpdateDateVersion();
            }).AddTo(ref _disposables);

            Inventory = new InventoryRoot(gameState.Inventory);
            ContainerChests = new ContainerChests(gameState.ContainerChests);
            CurrentMapId.Subscribe(newValue =>
            {
                gameState.CurrentMapId = newValue;
                UpdateDateVersion();
            }).AddTo(ref _disposables);

            MapStates = new MapStates(gameState.MapStatesData);

            MapStates.UpdateData.Subscribe(_ => UpdateDateVersion()).AddTo(ref _disposables);
            Inventory.UpdateData.Subscribe(_ => UpdateDateVersion()).AddTo(ref _disposables);
            ContainerChests.UpdateData.Subscribe(_ => UpdateDateVersion()).AddTo(ref _disposables);

        }

        public int CreateInventoryID()
        {
            return Origin.CreateInventoryID();
        }

        public bool PaidHardCurrency(int value)
        {
            //TODO Возможно вызвать событие @Нехватка денег@
            if (HardCurrency.CurrentValue < value) return false;
            HardCurrency.Value -= value;
            //Сохранить данные
            return true;
        }

        private void UpdateDateVersion()
        {
            Origin.DateVersion = DateTime.Now;
        }

        public void Dispose()
        {
            _disposables.Dispose();
            CurrentMapId?.Dispose();
            GameSpeed?.Dispose();
            HardCurrency?.Dispose();
            SoftCurrency?.Dispose();
            ContainerChests?.Dispose();
            MapStates?.Dispose();
            Inventory?.Dispose();
        }
    }
}