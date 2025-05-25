
using System;
using System.Collections.Generic;
using Game.State.Entities.Buildings;

namespace Game.State.Root
{
    [Serializable]
    public class GameState
    {
        public int GlobalEntityId;
        public List<BuildingEntity> Buildings;

    }    
}
