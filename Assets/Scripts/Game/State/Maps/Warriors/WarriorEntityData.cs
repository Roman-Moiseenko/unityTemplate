using UnityEngine;

namespace Game.State.Maps.Warriors
{
    public class WarriorEntityData
    {
        public int UniqueId { get; set; }
        public string ConfigId { get; set; }
        public Vector3 Position { get; set; } 
        public float Health { get; set; } 
        public float MaxHealth { get; set; } 
        public float Speed { get; set; } 
        public float Damage { get; set; } 
        public float Range { get; set; }
        public bool IsFly { get; set; }
        public int ParentId { get; set; } //Родительская башня
        public Vector3 StartPosition { get; set; }
    }
}