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
        private readonly ReactiveProperty<bool> _isDirection = new();
        private const int Speed = 20;
        private const float SmoothTime = 0.2f;
        private Vector3 _velocity;
        private IDisposable _disposable;
        private const string AnimationFireName = "tower_fire";

        private Quaternion _targetRotation;
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
            
            _disposable = d.Build();
        }

        private IEnumerator PauseTowerFire()
        {
            //Продолжить анимацию после окончания выстрела в сек
            //   yield return new WaitForSeconds(awayFire);
            //animator.SetBool("IsFire", _viewModel.IsShot.CurrentValue);
            yield return null;
        }

        public ReactiveProperty<bool> Direction(Vector2 newValue)
        {
            if (rotateBlock != null) //Вращение башни
            {
                var fromDirection = new Vector3(_viewModel.Position.Value.x, 0, _viewModel.Position.Value.y);
                var toDirection = new Vector3(newValue.x, 0, newValue.y);
                var direction = toDirection - fromDirection;
                _targetRotation = Quaternion.LookRotation(direction);
                _isDirection.OnNext(true);
            }

            return _isDirection;
        }

        public void FireAnimation()
        {
            if (animator == null) return;
            animator.Play(AnimationFireName);
        }

        private void Update()
        {
            if (!_isMoving) return;
            transform.position =
                Vector3.SmoothDamp(transform.position, _targetPosition, ref _velocity, SmoothTime, Speed);
            if (!(_velocity.magnitude < 0.0005)) return;
            
            _isMoving = false;
            transform.position = _targetPosition;
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }

        public IEnumerator StartDirection(Vector2 newValue)
        {
            if (rotateBlock == null) yield break; //Вращение башни

            var fromDirection = new Vector3(_viewModel.Position.Value.x, 0, _viewModel.Position.Value.y);
            var toDirection = new Vector3(newValue.x, 0, newValue.y);
            var direction = toDirection - fromDirection;
            _targetRotation = Quaternion.LookRotation(direction);
            _isDirection.OnNext(true);

            while (_isDirection.CurrentValue)
            {
                if (rotateBlock == null) //При смене модели башни
                {
                    _isDirection.OnNext(false);
                    _timeElapsed = 0;
                    yield break;
                }

                if (_timeElapsed < _viewModel.SpeedFire.Value)
                {
                    rotateBlock.transform.rotation = Quaternion.Lerp(rotateBlock.transform.rotation, _targetRotation,
                        _timeElapsed / _viewModel.SpeedFire.Value);
                    _timeElapsed += Time.deltaTime;
                }
                else
                {
                    _isDirection.OnNext(false);
                    _timeElapsed = 0;
                }

                yield return null;
            }
        }
    }
}