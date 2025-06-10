namespace Game.MainMenu.Root
{
    public class MainMenuEnterParams
    {
        public string Result { get; }
        public int SoftCurrency { get; set; }
        
        public bool HasSessionGame { get; set; }

        public MainMenuEnterParams(string result)
        {
            SoftCurrency = 0;
            Result = result;
        }
    }
}