using System;
using Game.State.Maps.Grounds;
using Game.State.Maps.Roads;
using Game.State.Maps.Towers;
using Game.State.Mergeable.Buildings;
using Game.State.Mergeable.ResourcesEntities;

namespace Game.State.Entities
{
    public static class EntitiesFactory
    {
        public static Entity CreateEntity(EntityData entityData)
        {
            switch (entityData.Type)
            {
                case EntityType.Building:
                    return new BuildingEntity(entityData as BuildingEntityData);
                case EntityType.Resource:
                    return new ResourcesEntity(entityData as ResourcesEntityData);
                case EntityType.Base:
            //        return new BaseEntity(entityData as BaseEntityData);
                default:
                    throw new Exception($"Unsupported entity type: " + entityData.Type);
            }
        }
    }
}