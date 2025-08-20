using Game.GameRoot;
using Scripts.Game.GameRoot;

namespace Game.MainMenu.Root
{
    public class MainMenuExitParams
    {
        
        public SceneEnterParams TargetSceneEnterParams;

        public MainMenuExitParams(SceneEnterParams targetSceneEnterParams)
        {
            TargetSceneEnterParams = targetSceneEnterParams;
        }
    }
}