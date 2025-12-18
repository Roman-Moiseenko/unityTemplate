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

            var meshRenderer = place.GetComponent<MeshRenderer>();
            var matBlock = new MaterialPropertyBlock();
            meshRenderer.GetPropertyBlock(matBlock);
            
            matBlock.SetTexture("_Plane", imageManager.GetGround(viewModel.ConfigId)); //viewModel.ConfigId
            matBlock.SetInt("_Odd", position);
            
            meshRenderer.SetPropertyBlock(matBlock);
            transform.position = new Vector3(
                viewModel.Position.CurrentValue.x,
                -1,
                viewModel.Position.CurrentValue.y
            );

            
        }
    }
}