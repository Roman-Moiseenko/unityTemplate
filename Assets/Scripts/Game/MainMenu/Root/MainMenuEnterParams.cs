namespace Game.MainMenu.Root
{
    public class MainMenuEnterParams
    {
        public string Result { get; }
        public int SoftCurrency { get; set; }
        
        public int GameSpeed { get; set; }
        public bool HasSessionGame { get; set; }

        public MainMenuEnterParams(string result)
        {
            GameSpeed = 1;
            SoftCurrency = 0;
            Result = result;
        }
    }
}