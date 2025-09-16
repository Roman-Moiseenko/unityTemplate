using Game.MainMenu.Services;
using Game.State.Inventory.Chests;
using Game.State.Root;
using MVVM.CMD;
using UnityEngine;

namespace Game.MainMenu.Commands.ChestCommands
{
    public class CommandChestOpenedHandler: ICommandHandler<CommandChestOpened>
    {
        
        private GameStateProxy _gameState;

        public CommandChestOpenedHandler(GameStateProxy gameState)
        {
            _gameState = gameState;
        }
        public bool Handle(CommandChestOpened command)
        {
            Debug.Log("сундук можно открыть бесплатно");
            //Меняем статус сундука на готов к открытию
            var chest = _gameState.ContainerChests.OpeningChest();
            
            chest.Status.OnNext(StatusChest.Opened);
            //Устанавливаем таймер и открывающийся сундук в 0
            //_gameState.ContainerChests.StartOpening.OnNext(0);
            _gameState.ContainerChests.CellOpening.OnNext(0);
            return true; //Сохраняем
        }
    }
}