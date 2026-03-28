using Game.State.Common;
using Game.State.Inventory.Common;

namespace Game.State.Inventory.HeroCards
{
    public class HeroCardData : InventoryItemData
    {
        public TypeEpic EpicLevel; //Возможно поменять на тип геройской эпичности
        public int Level;
        //TODO Добавить параметры героя public Dictionary<HeroParameterType, HeroParameterData> Parameters;
        public override bool Accumulation => true;
        public override InventoryType TypeItem => InventoryType.HeroCard;
    }
}