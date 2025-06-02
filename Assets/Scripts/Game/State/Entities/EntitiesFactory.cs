using System;
using Game.State.Mergeable.Buildings;
using Game.State.Mergeable.ResourcesEntities;

namespace Game.State.Entities
{
    public static class EntitiesFactory
    {
        public static Entity CreateEntity(EntityData entityData)
        {
            //TODO Добавить новые типы сущностей
            switch (entityData.Type)
            {
                case EntityType.Building:
                    return new BuildingEntity(entityData as BuildingEntityData);
                case EntityType.Resource:
                    return new ResourcesEntity(entityData as ResourcesEntityData);
                case EntityType.Tower:
        //            return new TowerEntity(entityData as TowerEntityData);
                case EntityType.Road:
          //          return new RoadEntity(entityData as RoadEntityData);
                case EntityType.Base:
            //        return new BaseEntity(entityData as BaseEntityData);
                default:
                    throw new Exception($"Unsupported entity type: " + entityData.Type);
            }
        }
    }
}