using System;
using System.Collections.Generic;
using Game.State.Entities;

namespace Game.State.Maps
{
 //   [Serializable]
    public class MapData
    {
        public int Id { get; set; }
        public List<EntityData> Entities;
    }
}