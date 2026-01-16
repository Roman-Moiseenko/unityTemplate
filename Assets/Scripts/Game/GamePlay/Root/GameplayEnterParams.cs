using System.Collections.Generic;
using Game.GamePlay.Classes;
using Game.GameRoot;
using Game.State.Inventory.TowerCards;
using Scripts.Game.GameRoot;

namespace Game.GamePlay.Root
{
    public class GameplayEnterParams :SceneEnterParams
    {
        public int MapId { get; }
        public float GameSpeed { get; set; }

        public TypeGameplay TypeGameplay { get; }
        //public bool HasSessionGameplay { get; set; }
        //public float DamageTowerBust { get; set; }
        public List<TowerCardData> Towers { get; } = new();

        public GameplayEnterParams(TypeGameplay typeGameplay, int uniqueId) : base(Scenes.GAMEPLAY)
        {
            TypeGameplay = typeGameplay;
            GameSpeed = 1;
            MapId = uniqueId;
            //Towers = towers;
            //  HasSessionGameplay = false;
        }
    }
}