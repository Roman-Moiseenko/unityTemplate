using Scripts.Game.GameRoot;

namespace Game.GamePlay.Root
{
    public class GameplayEnterParams :SceneEnterParams
    {
        public int MapId { get; }
        public int GameSpeed { get; set; }
        //public bool HasSessionGameplay { get; set; }
        public float DamageTowerBust { get; set; }
        
        public GameplayEnterParams(int mapId) : base(Scenes.GAMEPLAY)
        {
            GameSpeed = 1;
            MapId = mapId;
          //  HasSessionGameplay = false;
        }
    }
}