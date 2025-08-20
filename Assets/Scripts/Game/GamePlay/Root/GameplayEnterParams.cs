using System.Collections.Generic;
using Game.GameRoot;
using Game.State.Inventory.TowerCards;
using Scripts.Game.GameRoot;

namespace Game.GamePlay.Root
{
    public class GameplayEnterParams :SceneEnterParams
    {
        public int MapId { get; }
        public int GameSpeed { get; set; }
        //public bool HasSessionGameplay { get; set; }
        //public float DamageTowerBust { get; set; }
        public List<TowerCardData> Towers { get; } = new();

        public GameplayEnterParams(int mapId) : base(Scenes.GAMEPLAY)
        {
            GameSpeed = 1;
            MapId = mapId;
            //Towers = towers;
            //  HasSessionGameplay = false;
        }
    }
}