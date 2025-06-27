using R3;

namespace Game.State.Inventory.HeroCards
{
    public class HeroCard : InventoryItem
    {
        public ReactiveProperty<TypeEpicCard> EpicLevel;
        public readonly ReactiveProperty<int> Level;
        //TODO Добавить параметры героя public ObservableDictionary<HeroParameterType, HeroParameterData> Parameters;
        public HeroCard(HeroCardData data) : base(data)
        {
            EpicLevel = new ReactiveProperty<TypeEpicCard>(data.EpicLevel);
            EpicLevel.Subscribe(newValue => data.EpicLevel = newValue);
            
            Level = new ReactiveProperty<int>(data.Level);
            Level.Subscribe(newAmount => data.Level = newAmount);
        }
    }
}