using System.Collections.Generic;
using Game.State.Maps.Mobs;

namespace Game.State.Maps.Waves
{
    public class WaveEntityData
    {
        public int Number;
        public List<MobEntityData> Mobs; //<ConfigId, Quantity> 
    }
}