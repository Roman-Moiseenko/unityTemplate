using System;
using System.Reflection;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Shots
{
    public class ShotBinder : MonoBehaviour
    {
        public ShotViewModel _viewModel;
		private Vector3 _targetPosition;
        IDisposable disposable;
        public ReactiveProperty<bool> Free = new(true); //Доступность в пуле

        public void Bind(ShotViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            Free.Value = false;
            _viewModel = viewModel;
            transform.position = viewModel.StartPosition;
            viewModel.Position.Subscribe(p =>
            {
                transform.position = p;
            }).AddTo(ref d);

            _viewModel.Rotation.Subscribe(r => transform.rotation = r).AddTo(ref d);
            disposable = d.Build();
            gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            disposable.Dispose();
            //_viewModel.Dispose();
        }
        
        public void FreeUp()
        {
            gameObject.SetActive(false);
            Free.OnNext(true);
            disposable.Dispose();
        }
    }
}