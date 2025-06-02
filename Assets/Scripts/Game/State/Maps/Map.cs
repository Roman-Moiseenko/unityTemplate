using System.Linq;
using Game.State.Entities;
using ObservableCollections;
using R3;


namespace Game.State.Maps
{
    public class Map
    {
        public int Id => Origin.Id;
        private MapData _mapData;
        public MapData Origin { get; }
        public ObservableList<Entity> Entities { get; } = new();

        public Map(MapData mapData)
        {
            Origin = mapData;
            mapData.Entities.ForEach(
                entityOriginal => Entities.Add(EntitiesFactory.CreateEntity(entityOriginal)));
            Entities.ObserveAdd().Subscribe(e =>
            {
                var addedEntity = e.Value;
                mapData.Entities.Add(addedEntity.Origin);
            });

            Entities.ObserveRemove().Subscribe(e =>
            {
                var removedEntity = e.Value;
                var removedEntityData =
                    mapData.Entities.FirstOrDefault(b => b.UniqueId == removedEntity.UniqueId);
                mapData.Entities.Remove(removedEntityData);
            });
        }
    }
}