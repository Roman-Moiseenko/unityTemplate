using System;

namespace Game.State.GameResources
{
	//[Serializable]
    public class ResourceData
    {
        public ResourceType ResourceType { get; set; }
		public int Amount { get; set; }
    }
}