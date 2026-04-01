using System.Collections.Generic;
using Game.State.Inventory.HeroCards;
using Game.State.Inventory.SkillCards;
using Game.State.Inventory.TowerCards;

namespace Game.State.Inventory.Deck
{
    public class DeckCardData
    {
        public readonly List<int> TowerCardIds = new();
        public readonly List<int> SkillCardIds = new();
        public int HeroCardId ;
        
    }
}