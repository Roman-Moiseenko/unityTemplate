using System.Reflection;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Shots
{
    public class ShotBinder : MonoBehaviour
    {
        private ShotViewModel _viewModel;
		private Vector3 _targetPosition;

        public void Bind(ShotViewModel viewModel)
        {
            _viewModel = viewModel;
            transform.position = viewModel.StartPosition;
            viewModel.Position.Subscribe(p =>
            {
                transform.position = p;
            });

            _viewModel.Rotation.Subscribe(r => transform.rotation = r);
            //transform.rotation = 
        }
    }
}