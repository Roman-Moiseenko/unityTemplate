using Game.GamePlay.Classes;

namespace Game.State.Inventory.Chests
{
    public class ChestEntityData
    {
        public TypeChest TypeChest;
        public TypeGameplay Gameplay;
        public int MapId; //Id карты откуда брать данные о награде
        public int Level;
        public int Cell;
        public int Wave;
        public StatusChest Status;
    }
}