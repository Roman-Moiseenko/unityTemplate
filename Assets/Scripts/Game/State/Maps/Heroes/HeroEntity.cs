using System;

namespace Game.State.Maps.Heroes
{
    public class HeroEntity : IDisposable
    {
        public HeroEntityData Origin { get; }
        public int UniqueId => Origin.UniqueId;
        public string ConfigId => Origin.ConfigId;

        public HeroEntity(HeroEntityData heroEntityData)
        {
            Origin = heroEntityData;
            
            
        }
        
        
        public void Dispose()
        {
            
        }
    }
}