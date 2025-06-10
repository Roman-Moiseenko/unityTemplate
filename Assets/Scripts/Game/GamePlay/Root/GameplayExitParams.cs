using Game.MainMenu.Root;

namespace Game.GamePlay.Root
{
    public class GameplayExitParams
    {
        public MainMenuEnterParams MainMenuEnterParams { get; }
        public bool SaveGameplay { get; set; } 
        
        public GameplayExitParams(MainMenuEnterParams mainMenuEnterParams)
        {
            SaveGameplay = false;
            MainMenuEnterParams = mainMenuEnterParams;
        }
    }
}