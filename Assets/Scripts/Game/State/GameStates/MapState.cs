using System;
using Game.State.Inventory.Chests;
using ObservableCollections;
using R3;

namespace Game.State.GameStates
{
    public class MapState
    {
        public MapStateData Origin { get; set; }
        public int MapId => Origin.MapId;
        public ReactiveProperty<bool> Finished;
        public int LastWave = 0;
        public ObservableList<ResultMap> Results;
        
        public ReactiveProperty<int> RewardOnWave;
        public ReactiveProperty<TypeChest> RewardChest;
        
        public MapState(MapStateData mapStateData)
        {
            Origin = mapStateData;
            Finished = new ReactiveProperty<bool>(mapStateData.Finished);
            Finished.Where(x => x).Subscribe(_ => mapStateData.Finished = true);

            RewardOnWave = new ReactiveProperty<int>(mapStateData.RewardOnWave);
            RewardOnWave.Subscribe(v => mapStateData.RewardOnWave = v);

            RewardChest = new ReactiveProperty<TypeChest>(mapStateData.RewardChest);
            RewardChest.Subscribe(v => mapStateData.RewardChest = v);

            Results = new ObservableList<ResultMap>();
            foreach (var resultMap in mapStateData.Results)
            {
                Results.Add(resultMap);
                LastWave = Math.Max(LastWave, resultMap.LastWave);
            }

            Results.ObserveAdd().Subscribe(e =>
            {
                mapStateData.Results.Add(e.Value);
                LastWave = Math.Max(LastWave, e.Value.LastWave);
            });
        }

        
    }
}