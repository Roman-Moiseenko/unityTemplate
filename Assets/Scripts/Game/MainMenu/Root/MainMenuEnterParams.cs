using System.Collections.Generic;

namespace Game.MainMenu.Root
{
    public class MainMenuEnterParams
    {
        public string Result { get; }
        public int SoftCurrency { get; set; }
        
        public int LastWave { get; set; } //Последняя волна, при проигрыше
        public bool CompletedLevel { get; set; } //Уровень выйгран
        
        public int GameSpeed { get; set; }
        public int MapId { get; set; }

        public Dictionary<string, int> RewardCards = new (); //Список карт - наград
        

        public MainMenuEnterParams(string result)
        {
            GameSpeed = 1;
            SoftCurrency = 0;
            Result = result;
        }
    }
}