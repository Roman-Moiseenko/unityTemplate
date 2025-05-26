using System;
using System.Collections.Generic;
using Game.State.Entities.Buildings;

namespace Game.State.Maps
{
    [Serializable]
    public class MapState
    {
        public int Id;
        public List<BuildingEntity> Buildings;
    }
}