using System.Collections.Generic;
using Game.State.Inventory.HeroCards;
using Game.State.Inventory.SkillCards;
using Game.State.Inventory.TowerCards;

namespace Game.State.Inventory.Deck
{
    public class DeckCardData
    {
        public Dictionary<int, int> TowerCardIds = new Dictionary<int, int> ();
        public Dictionary<int, int> SkillCardIds = new Dictionary<int, int>();
        public int HeroCardId ;
        
    }
}