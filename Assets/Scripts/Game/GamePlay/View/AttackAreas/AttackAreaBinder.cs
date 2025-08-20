using System;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.AttackAreas
{
    public class AttackAreaBinder : MonoBehaviour
    {
        [SerializeField] private Transform _area;
        [SerializeField] private Transform _areaDisabled;
        [SerializeField] private Transform _areaExpansion;
        private AttackAreaViewModel _viewModel;

        private Vector3 _targetPosition;
        private bool _isMoving = false;
        private const int speed = 20;
        private const float smoothTime = 0.2f;
        private Vector3 _velocity;
        private IDisposable _disposable;

        public void Bind(AttackAreaViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            _viewModel = viewModel;
            transform.position = new Vector3(viewModel.Position.CurrentValue.x, 0, viewModel.Position.CurrentValue.y);
            _viewModel.Position.Subscribe(newPosition =>
            {
                if (_viewModel.Moving)
                {
                    _targetPosition = new Vector3(newPosition.x, 0, newPosition.y);
                    _isMoving = true;
                }
                else
                {
                    transform.position = new Vector3(newPosition.x, 0, newPosition.y);
                }
            }).AddTo(ref d);
            _viewModel.RadiusArea.Subscribe(r => _area.transform.localScale = GetDimensions(r)).AddTo(ref d);
            _viewModel.RadiusDisabled.Subscribe(r => _areaDisabled.transform.localScale = GetDimensions(r))
                .AddTo(ref d);
            _viewModel.RadiusExpansion.Subscribe(r => _areaExpansion.transform.localScale = GetDimensions(r))
                .AddTo(ref d);
            _disposable = d.Build();
        }


        private void Update()
        {
            if (!_isMoving) return;
            
            transform.position =
                Vector3.SmoothDamp(transform.position, _targetPosition, ref _velocity, smoothTime, speed);
            if (_velocity.magnitude < 0.0005)
            {
                _isMoving = false;
                transform.position = _targetPosition;
            }
        }


        private Vector3 GetDimensions(float radius)
        {
            var r = radius == 0 ? 0 : 1f + 2 * radius;
            return new Vector3(r, r, 1);
        }

        private void OnDestroy()
        {
            _disposable.Dispose();
        }
    }
}