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
        private bool _isDirection = false;
        private const int Speed = 20;
        private const float SmoothTime = 0.2f;
        private Vector3 _velocity;
        private IDisposable _disposable;
        private const string AnimationFireName = "tower_fire";

        private Quaternion _targetRotation;
        private float _timeElapsed;
        private float _lerpDuration;
        private readonly Vector3 _baseScale = new Vector3(0.7f, 0.7f, 0.7f);

        private Vector3 _targetScaled;
        private bool _isScaled = false;
        
        private ReactiveProperty<bool> ResultRemoveAnimation = new(false);
        private void Awake()
        {
           // animator?.StopPlayback();
        }

        
        public void Bind(TowerViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            _viewModel = viewModel;
            viewModel.Position.Subscribe(newPosition =>
            {
                _targetPosition = new Vector3(newPosition.x, transform.position.y, newPosition.y);
                _isMoving = true;
            }).AddTo(ref d);

            Debug.Log("animator " + animator);
            if (viewModel.IsUpdate)
            {
                transform.localScale = Vector3.zero;
                _targetScaled = _baseScale;
                _isScaled = true;
                Debug.Log("Запускаем анимацию появления башни");
                //TODO Запускаем анимацию увеличения
                //TODO и включаем шейдер levelUp
                viewModel.IsUpdate = false; // На всякий случай
            }
            transform.position = new Vector3(
                viewModel.Position.CurrentValue.x,
                transform.position.y,
                viewModel.Position.CurrentValue.y
            );
           
            viewModel.Direction.Skip(1).Subscribe(newValue =>
            {
                if (rotateBlock != null) //Вращение башни
                {
                    var fromDirection = new Vector3(viewModel.Position.Value.x, 0, viewModel.Position.Value.y);
                    var toDirection = new Vector3(newValue.x, 0, newValue.y);
                    var direction = toDirection - fromDirection;
                    _targetRotation = Quaternion.LookRotation(direction);
                    _isDirection = true;
                }
            }).AddTo(ref d);

            if (animator != null)
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
            _disposable = d.Build();
        }

        private IEnumerator PauseTowerFire()
        { //Продолжить анимацию после окончания выстрела в сек
            yield return new WaitForSeconds(awayFire); 
            animator.SetBool("IsFire", _viewModel.IsShot.CurrentValue);
        }
        
        private void Update()
        {
            if (_isMoving)
            {
                transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref _velocity, SmoothTime, Speed );
                if (_velocity.magnitude < 0.0005)
                {
                    _isMoving = false;
                    transform.position = _targetPosition;
                }
            }
            if (_isDirection)
            {
                if (_timeElapsed < _viewModel.SpeedFire.Value)
                {
                    rotateBlock.transform.rotation = Quaternion.Lerp(rotateBlock.transform.rotation, _targetRotation,_timeElapsed / _viewModel.SpeedFire.Value);
                    _timeElapsed += Time.deltaTime;
                }
                else
                {
                    _isDirection = false;
                    _timeElapsed = 0;
                }
            }

            if (_isScaled)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, _targetScaled, 0.20f);
                if (Math.Abs(transform.localScale.x - _targetScaled.x) < 0.1 )
                {
                    transform.localScale = _targetScaled;
                    _isScaled = false;
                    ResultRemoveAnimation.OnNext(true); //Анимация уменьшения закончена - удаляем модель
                }
            }
        }
        
        private void OnDestroy()
        {
            _disposable?.Dispose();
        }

        public ReactiveProperty<bool> RemoveTowerAnimation()
        {
            Debug.Log("TowerBinder -  Запустили анимацию ");
            //TODO Запуск анимации исчезания
            _isScaled = true;
            _targetScaled = Vector3.zero;
            
            //TODO Запуск шейдера исчезания
            
            return ResultRemoveAnimation;
        }
    }
    
    
}