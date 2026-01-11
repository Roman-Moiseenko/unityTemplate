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
        [SerializeField] private float awayFire = 0f;
        private TowerViewModel _viewModel;
        private Vector3 _targetPosition;
        private bool _isMoving = false;
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

            //_isDirection.Subscribe(rotate => )
            /*
            viewModel.Direction.Skip(1).Subscribe(newValue =>
            {
                if (rotateBlock != null) //Вращение башни
                {
                    var fromDirection = new Vector3(viewModel.Position.Value.x, 0, viewModel.Position.Value.y);
                    var toDirection = new Vector3(newValue.x, 0, newValue.y);
                    var direction = toDirection - fromDirection;
                    _targetRotation = Quaternion.LookRotation(direction);
                    _isDirection.Value = true;
                }
            }).AddTo(ref d);
*/
            /* if (animator != null)
             {
                 viewModel.IsShot.Subscribe(v =>
                 {
                     switch (v)
                     {
                         case true:
                             animator.SetBool("IsFire", true);
                             //Debug.Log(viewModel.Position.CurrentValue);
                             break;
                         case false:
                             //Debug.Log("StartCoroutine");
                             StartCoroutine(PauseTowerFire());
                             break;
                     }
                 }).AddTo(ref d);
             }
             */
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
          //  Debug.Log("Анимция " + _viewModel.TowerEntityId);

          animator.Play(AnimationFireName);
            //animator.SetBool("IsFire", true);
           // StartCoroutine(PauseTowerFire());
        }

        private void Update()
        {
            if (_isMoving)
            {
                transform.position =
                    Vector3.SmoothDamp(transform.position, _targetPosition, ref _velocity, SmoothTime, Speed);
                if (_velocity.magnitude < 0.0005)
                {
                    _isMoving = false;
                    transform.position = _targetPosition;
                }
            }
/*
            if (_isDirection.CurrentValue)
            {
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
            }
            */
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