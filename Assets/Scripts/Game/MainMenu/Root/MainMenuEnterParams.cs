using System.Collections.Generic;
using Game.GamePlay.Classes;
using Game.State.Inventory;
using Game.State.Inventory.Chests;
using Game.State.Maps.Rewards;

namespace Game.MainMenu.Root
{
    public class MainMenuEnterParams
    {
        public string Result { get; }
        public long SoftCurrency { get; set; }
        
        public int LastWave { get; set; } //Последняя волна, при проигрыше
        public bool CompletedLevel { get; set; } //Уровень выйгран
        
        public int GameSpeed { get; set; }
        public int MapId { get; set; }
        public int KillsMob { get; set; }
        public TypeGameplay TypeGameplay { get; set; }

        public int LastRewardOnWave { get; set; }
        public TypeChest? LastRewardChest { get; set; }
        /**
         * Награда сундуком, № ячейки, или 0 если нет места
         */
        public TypeChest? TypeChest { get; set; }

        public List<InventoryItemData> Items = new (); 
        public List<RewardEntityData> RewardCards = new (); //Список карт - наград
        public List<RewardEntityData> RewardOnWave = new (); //Список карт - наград


        public MainMenuEnterParams(string result)
        {
            GameSpeed = 1;
            SoftCurrency = 0;
            Result = result;
        }
        
    }
}