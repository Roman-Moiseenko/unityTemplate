using System;
using System.Reflection;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Shots
{
    public class ShotBinder : MonoBehaviour
    {
        private ShotViewModel _viewModel;
		private Vector3 _targetPosition;
        IDisposable disposable;
        
        public void Bind(ShotViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();

            _viewModel = viewModel;
            transform.position = viewModel.StartPosition;
            viewModel.Position.Subscribe(p =>
            {
                transform.position = p;
            }).AddTo(ref d);

            _viewModel.Rotation.Subscribe(r => transform.rotation = r).AddTo(ref d);
            disposable = d.Build();
        }

        private void OnDestroy()
        {
            disposable.Dispose();
        }
    }
}