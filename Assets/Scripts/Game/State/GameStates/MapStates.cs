using System;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;

namespace Game.State.GameStates
{
    public class MapStates : IDisposable
    {
        public ReactiveProperty<int> LastMap;
        public ObservableDictionary<int, MapState> Maps;

        public Subject<Unit> UpdateData = new();
        private DisposableBag _disposables = new();

        public MapStates(MapStatesData mapStatesData)
        {
            LastMap = new ReactiveProperty<int>(mapStatesData.LastMap);
            LastMap.Subscribe(v =>
            {
                mapStatesData.LastMap = v;
                UpdateData.OnNext(Unit.Default);
            }).AddTo(ref _disposables);

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
            }).AddTo(ref _disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
            LastMap?.Dispose();
            UpdateData?.Dispose();
        }
    }
}