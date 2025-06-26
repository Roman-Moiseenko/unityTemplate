using Game.State.Maps.Towers;
using ObservableCollections;
using R3;

namespace Game.State.Inventory.TowerCard
{
    public class TowerCard : Inventory
    {
        //public TowerType TowerType { get; }
        public ReactiveProperty<int> EpicLevel;
        public ObservableDictionary<TowerParameterType, TowerParameter> Parameters;

        public TowerCard(TowerCardData data) : base(data)
        {
            EpicLevel = new ReactiveProperty<int>(data.EpicLevel);
            EpicLevel.Subscribe(newValue => data.EpicLevel = newValue);
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