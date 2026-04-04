using System.Collections.Generic;
using Game.State.Common;
using Game.State.Inventory.Common;
using Game.State.Maps.Skills;

namespace Game.State.Inventory.SkillCards
{
    public class SkillCardData : InventoryItemData
    {
        public override bool Accumulation => false;
        public override InventoryType TypeItem => InventoryType.SkillCard;
        public TypeDefence Defence { get; set; }

        public TypeEpic EpicLevel;
        public TypeTarget TypeTarget;
        public int Level;
        public Dictionary<SkillParameterType, SkillParameterData> Parameters;
    }
}