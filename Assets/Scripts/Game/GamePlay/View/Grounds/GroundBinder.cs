using System;
using Game.Common;
using Game.GameRoot.ImageManager;
using UnityEngine;

namespace Game.GamePlay.View.Grounds
{
    public class GroundBinder : MonoBehaviour
    {

        [SerializeField] private GameObject place;
        public void Bind(GroundViewModel viewModel)
        {
            var imageManager = GameObject.Find(AppConstants.IMAGE_MANAGER).GetComponent<ImageManagerBinder>();
            var position = Math.Abs((viewModel.Position.CurrentValue.x + viewModel.Position.CurrentValue.y) % 2);
            var material = place.GetComponent<Renderer>().material;
            
            material.SetTexture("_Plane", imageManager.GetGround(viewModel.ConfigId)); //viewModel.ConfigId
            material.SetInt("_Odd", position);
            
            transform.position = new Vector3(
                viewModel.Position.CurrentValue.x,
                -1,
                viewModel.Position.CurrentValue.y
            );

            
        }
    }
}