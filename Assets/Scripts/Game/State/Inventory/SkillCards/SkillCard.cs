using R3;

namespace Game.State.Inventory.SkillCards
{
    public class SkillCard : InventoryItem
    {
        public ReactiveProperty<TypeEpicCard> EpicLevel;
        public readonly ReactiveProperty<int> Level;
        
       //TODO public ObservableDictionary<SkillParameterType, SkillParameterData> Parameters;
        public SkillCard(SkillCardData data) : base(data)
        {
            EpicLevel = new ReactiveProperty<TypeEpicCard>(data.EpicLevel);
            EpicLevel.Subscribe(newValue => data.EpicLevel = newValue);
            
            
            Level = new ReactiveProperty<int>(data.Level);
            Level.Subscribe(newAmount => data.Level = newAmount);
        }
    }
}