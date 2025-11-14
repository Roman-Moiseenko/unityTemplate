using ObservableCollections;
using R3;

namespace Game.State.GameStates
{
    public class MapStates
    {
        public ReactiveProperty<int> LastMap;
        public ObservableDictionary<int, MapState> Maps;

        public Subject<Unit> UpdateData = new();
        
        public MapStates(MapStatesData mapStatesData)
        {
            LastMap = new ReactiveProperty<int>(mapStatesData.LastMap);
            LastMap.Subscribe(v =>
            {
                mapStatesData.LastMap = v;
                UpdateData.OnNext(Unit.Default);
            });

            Maps = new ObservableDictionary<int, MapState>();

            foreach (var (index, stateData) in mapStatesData.Maps)
            {
                Maps.Add(index, new MapState(stateData));
            }

            Maps.ObserveAdd().Subscribe(e =>
            {
                var newValue = e.Value;
                mapStatesData.Maps.Add(newValue.Key, newValue.Value.Origin);
                UpdateData.OnNext(Unit.Default);
            });
        }
    }
}