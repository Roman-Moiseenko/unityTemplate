using Game.GamePlay.Classes;
using Game.State.Inventory.Chests;
using MVVM.CMD;

namespace Game.MainMenu.Commands.ChestCommands
{
    public class CommandChestAdd: ICommand
    {
        public int LevelChest;
        public TypeChest TypeChest;
        public TypeGameplay Gameplay;
        public int Wave;
    }
}