using System;
using UnityEngine;

namespace Game.State.Entities.Buildings
{
    [Serializable]
    public class BuildingEntity : Entity
    {
        public string TypeId;
        public Vector3Int Position;
        public int Level;

    }
}