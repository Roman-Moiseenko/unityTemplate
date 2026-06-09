using System.Collections.Generic;
using Game.State.Common;
using Game.State.Inventory.Common;
using Game.State.Parameters;

namespace Game.State.Inventory.HeroCards
{
    public class HeroCardData : InventoryItemData
    {
        public TypeEpic EpicLevel; //Неизменный,ни начто не влияет 
        public bool Available;
        public int Level;
        public int Rank { get; set; } //от 1 до 7
        
        public readonly Dictionary<ParameterType, ParameterData> Parameters = new();
        public override bool Accumulation => true;
        public override InventoryType TypeItem => InventoryType.HeroCard;
        public TypeDefence Defence { get; set; }
    }
}