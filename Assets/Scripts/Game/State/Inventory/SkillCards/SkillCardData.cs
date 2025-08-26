namespace Game.State.Inventory.SkillCards
{
    public class SkillCardData : InventoryItemData
    {
        public override bool Accumulation => false;
        public override InventoryType TypeItem => InventoryType.SkillCard;

        public TypeEpicCard EpicLevel;
        public int Level;
        //TODO Добавить параметры навыков public Dictionary<SkillParameterType, SkillParameterData> Parameters;
    }
}