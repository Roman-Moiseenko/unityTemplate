using Cysharp.Threading.Tasks;
using Game.State.Common;
using Game.State.Inventory.Common;
using Game.State.Parameters;
using ObservableCollections;
using R3;

namespace Game.State.Inventory.HeroCards
{
    public class HeroCard : InventoryItem
    {
        public TypeEpic EpicLevel => ((HeroCardData)Origin).EpicLevel;
        public bool Available => ((HeroCardData)Origin).Available;
        public TypeDefence Defence => ((HeroCardData)Origin).Defence;
        
        
        public readonly ReactiveProperty<int> Level;
        public readonly ReactiveProperty<int> Rank;
        
        public ObservableDictionary<ParameterType, ParameterData> Parameters;
        public HeroCard(HeroCardData data) : base(data)
        {
            Level = new ReactiveProperty<int>(data.Level);
            Level
                .Subscribe(newAmount => data.Level = newAmount)
                .AddTo(ref _disposables);
            
            Rank = new ReactiveProperty<int>(data.Rank);
            Rank
                .Subscribe(newAmount => data.Rank = newAmount)
                .AddTo(ref _disposables);
        }
        
    }
}