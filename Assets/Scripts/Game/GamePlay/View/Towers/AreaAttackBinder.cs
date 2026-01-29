using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.GamePlay.View.Towers
{
    public class AreaAttackBinder : AreaBinder
    {
        
        private Material _material;

        private void Awake()
        {
            _material = area.gameObject.GetComponent<Renderer>().material;
        }

        protected override Vector3 SetDimensions(Vector3 radius)
        {
            var radiusVector = GetDimensions(radius.x + radius.x * radius.z);
            //_area.transform.localScale = radiusVector;
            var _d = GetDimensions(radius.y).x;
            var _e = GetDimensions(radius.z).x;
            
            _material.SetFloat("_Disabled", _d / radiusVector.x);
            _material.SetFloat("_Expansion", _e / radiusVector.x);
            _material.SetFloat("_Thickness", 0.04f / radiusVector.x); //Ободок
            
            return radiusVector;
        }
        
        private Vector3 GetDimensions(float radius)
        {
            var r = radius == 0 ? 0 : 1f + 2 * radius;
            return new Vector3(r, r, 1);
        }
    }
}