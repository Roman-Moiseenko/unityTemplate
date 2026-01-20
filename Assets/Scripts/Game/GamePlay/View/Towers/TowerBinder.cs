using System;
using System.Collections;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Towers
{
    public class TowerBinder : MonoBehaviour
    {
        [SerializeField] private Transform rotateBlock;
        [SerializeField] private Animator animator;
        [SerializeField] private float awayFire;
        private TowerViewModel _viewModel;
        private Vector3 _targetPosition;
        private bool _isMoving;
        private bool _isDirection;
        private const int Speed = 20;
        private const float SmoothTime = 0.2f;
        private Vector3 _velocity;
        private IDisposable _disposable;
        private const string AnimationFireName = "tower_fire";

        private Vector3 _targetDirection;
       // private Quaternion _targetRotation;
        private float _timeElapsed = 0f;
        private float _lerpDuration;

        public void Bind(TowerViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            _viewModel = viewModel;
            viewModel.Position.Subscribe(newPosition =>
            {
                _targetPosition = new Vector3(newPosition.x, transform.position.y, newPosition.y);
                _isMoving = true;
            }).AddTo(ref d);
            if (rotateBlock != null && viewModel.Direction.CurrentValue != Vector3.zero)
                rotateBlock.rotation = Quaternion.LookRotation(viewModel.Direction.CurrentValue);
            
            viewModel.Direction.Where(x => x != Vector3.zero).Subscribe(v =>
            {
                if (rotateBlock != null) rotateBlock.rotation = Quaternion.LookRotation(v);
                
            }).AddTo(ref d);
            _disposable = d.Build();
        }
        
        public void FireAnimation()
        {
            if (animator == null) return;
            animator.Play(AnimationFireName);
        }

        private void Update()
        {
            if (_isMoving)
            {
                transform.position =
                    Vector3.SmoothDamp(transform.position, _targetPosition, ref _velocity, SmoothTime, Speed);
                if (!(_velocity.magnitude < 0.0005)) return;

                _isMoving = false;
                transform.position = _targetPosition;
            }
            
            if (_isDirection)
            {
                if (rotateBlock == null) //При смене модели башни
                {
                    _isDirection = false;
                    _timeElapsed = 0;
                    return;
                }

                if (_timeElapsed < _viewModel.SpeedFire)
                {
                    _viewModel.Direction.Value = Vector3.Lerp(_viewModel.Direction.Value, _targetDirection,_timeElapsed / _viewModel.SpeedFire);
                    _timeElapsed += Time.deltaTime;
                }
                else
                {
                    _isDirection = false;
                    _timeElapsed = 0;
                }
            }
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }

        public void StartDirection(Vector2 newValue)
        {
            if (rotateBlock == null) return; //Вращение башни
            var fromDirection = new Vector3(_viewModel.Position.Value.x, 0, _viewModel.Position.Value.y);
            var toDirection = new Vector3(newValue.x, 0, newValue.y);
            _targetDirection = toDirection - fromDirection;
            _isDirection = true;
        }
    }
}