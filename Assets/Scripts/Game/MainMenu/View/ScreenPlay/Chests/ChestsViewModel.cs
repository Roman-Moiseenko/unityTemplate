using System.Collections.Generic;
using DI;
using Game.State.Inventory.Chests;
using Game.State.Root;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.MainMenu.View.ScreenPlay.Chests
{
    public class ChestsViewModel
    {
        public Dictionary<int, CellChestViewModel> CellsViewModel = new();  

        public ChestsViewModel(GameStateProxy gameState, DIContainer container)
        {
            //Создаем модели ячеек сундуков и подписываемся на сундуки
            //Инициализация моделей
            for (var i = 1; i <= GameStateProxy.MaxChest; i++)
            {
                if (gameState.ContainerChests.Chests.TryGetValue(i, out var chest))
                {
                    CellsViewModel.Add(i, new CellChestViewModel(gameState.ContainerChests, container, chest));
                }
                else
                {
                    CellsViewModel.Add(i, new CellChestViewModel(gameState.ContainerChests, container, null));
                }
                
            }
            //Заполняем модели сундуками
            /*
            foreach (var (cell, chest) in gameState.ContainerChests.Chests)
            {
                CellsViewModel[cell].SetChest(chest);
            }
            */
            gameState.ContainerChests.Chests.ObserveAdd().Subscribe(e =>
            {
                var cell = e.Value.Key;
                var chestEntity = e.Value.Value;
                CellsViewModel[cell].SetChest(chestEntity);
            });
            
            gameState.ContainerChests.Chests.ObserveRemove().Subscribe(e =>
            {
                var cell = e.Value.Key;
                CellsViewModel[cell].ClearCell();
            }); 
            
        }
    }
}