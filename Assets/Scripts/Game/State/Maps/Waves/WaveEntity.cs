using System.Collections.Generic;
using Game.State.Maps.Mobs;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.State.Maps.Waves
{
    public class WaveEntity
    {
        public readonly WaveEntityData Origin;
        public int Number => Origin.Number;
        public ObservableList<MobEntity> Mobs = new();
        //public 
        
        public WaveEntity(WaveEntityData waveEntityData)
        {
            Origin = waveEntityData;
            foreach (var mobEntityData in waveEntityData.Mobs)
            {
                Mobs.Add(new MobEntity(mobEntityData));
            }

            Mobs.ObserveAdd().Subscribe(e => waveEntityData.Mobs.Add(e.Value.Origin));
            Mobs.ObserveRemove().Subscribe(e => waveEntityData.Mobs.Remove(e.Value.Origin));

        }
    }
}