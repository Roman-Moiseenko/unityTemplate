using Game.State.Maps.Towers;
using ObservableCollections;
using R3;

namespace Game.State.Inventory.TowerCards
{
    public class TowerCard : InventoryItem
    {
        //public TowerType TowerType { get; }
        public ReactiveProperty<TypeEpicCard> EpicLevel;
        public readonly ReactiveProperty<int> Level;
        
        public ObservableDictionary<TowerParameterType, TowerParameter> Parameters;

        public TowerCard(TowerCardData data) : base(data)
        {
            EpicLevel = new ReactiveProperty<TypeEpicCard>(data.EpicLevel);
            EpicLevel.Subscribe(newValue => data.EpicLevel = newValue);
            
            
            Level = new ReactiveProperty<int>(data.Level);
            Level.Subscribe(newAmount => data.Level = newAmount);
            
            Parameters = new ObservableDictionary<TowerParameterType, TowerParameter>();
            foreach (var parameter in data.Parameters)
            {
                Parameters.Add(parameter.Key, new TowerParameter(parameter.Value));
            }

            Parameters.ObserveChanged().Subscribe(newValue =>
            {
                var ket = newValue.NewItem.Key;
                var value = newValue.NewItem.Value;
                //TODO Протестить, может и не понадобится
            });

            //     TowerType = data.TowerType;
        }
    }
}