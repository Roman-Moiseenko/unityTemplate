
using System;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Towers
{
    public class TowerBinder : MonoBehaviour
    {
        private TowerViewModel _viewModel;
        private Vector3 _targetPosition;
        private bool _isMoving = false;
        private const int speed = 20;
        private const float smoothTime = 0.2f;
        private Vector3 _velocity;
        private IDisposable _disposable;

        public void Bind(TowerViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            _viewModel = viewModel;
            _viewModel.Position.Subscribe(newPosition =>
            {
                _targetPosition = new Vector3(newPosition.x, 0, newPosition.y);
                _isMoving = true;
            }).AddTo(ref d);
            
            transform.position = new Vector3(
                viewModel.Position.CurrentValue.x,
                0,
                viewModel.Position.CurrentValue.y
            );
            _disposable = d.Build();
        }
        
        private void Update()
        {
            if (_isMoving)
            {
                transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref _velocity, smoothTime, speed );
                if (_velocity.magnitude < 0.0005)
                {
                    _isMoving = false;
                    transform.position = _targetPosition;
                }
            
            }
        }
        
        private void OnDestroy()
        {
            _disposable.Dispose();
        }
    }
    
    
}