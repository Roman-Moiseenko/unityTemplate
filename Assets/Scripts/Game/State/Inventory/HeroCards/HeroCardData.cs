namespace Game.State.Inventory.HeroCards
{
    public class HeroCardData : InventoryItemData
    {
        public TypeEpicCard EpicLevel; //Возможно поменять на тип геройской эпичности
        public int Level;
        //TODO Добавить параметры героя public Dictionary<HeroParameterType, HeroParameterData> Parameters;
        public override bool Accumulation => true;
    }
}