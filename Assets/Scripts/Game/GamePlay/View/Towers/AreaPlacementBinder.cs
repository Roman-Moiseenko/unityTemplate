using UnityEngine;

namespace Game.GamePlay.View.Towers
{
    public class AreaPlacementBinder : AreaBinder
    {
        protected override Vector3 SetDimensions(Vector3 radius)
        {
            //TODO Возможно брать из 
            //return new Vector3(5, 5, 1);
            return radius;
        }
    }
}