using System.Collections.Generic;
using Game.State.Inventory.HeroCards;
using Game.State.Inventory.SkillCards;
using Game.State.Inventory.TowerCards;

namespace Game.State.Inventory.Deck
{
    public class DeckCardData
    {
        public Dictionary<int, int> TowerCardIds = new();
        public Dictionary<int, int> SkillCardIds = new();
        public int HeroCardId ;
        
    }
}