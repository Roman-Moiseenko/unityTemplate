using System.Linq;
using Game.State.Entities.Buildings;
using ObservableCollections;
using R3;


namespace Game.State.Maps
{
    public class Map
    {
        public int Id;
        private MapState _mapState;
        public MapState Origin { get; }
        public ObservableList<BuildingEntityProxy> Buildings { get; } = new();

        public Map(MapState mapState)
        {
            Origin = mapState;
            Id = mapState.Id;
            mapState.Buildings.ForEach(
                buildingOriginal => Buildings.Add(new BuildingEntityProxy(buildingOriginal))
            );
            Buildings.ObserveAdd().Subscribe(e =>
            {
                var addedBuildingEntity = e.Value;
                mapState.Buildings.Add(addedBuildingEntity.Origin);
            });

            Buildings.ObserveRemove().Subscribe(e =>
            {
                var removedBuildingEntityProxy = e.Value;
                var removedBuildingEntity =
                    mapState.Buildings.FirstOrDefault(b => b.Id == removedBuildingEntityProxy.Id);
                mapState.Buildings.Remove(removedBuildingEntity);
            });
        }
    }
}