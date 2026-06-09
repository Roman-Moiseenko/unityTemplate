using System.Collections.Generic;
using Game.State.Common;
using Game.State.Inventory.Common;
using Game.State.Maps.Skills;
using Game.State.Parameters;

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
        public Dictionary<ParameterType, ParameterData> Parameters;
    }
}