using System;
using System.Collections;
using Game.Common;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Towers
{
    public class TowerShotBinder : MonoBehaviour
    {
        [SerializeField] private Transform fire;
        [SerializeField] private Transform missile;
        [SerializeField] private Transform explosion;

        
        private IDisposable _disposable;
        private TowerViewModel _viewModel;
        private ReactiveProperty<bool> _isMoving = new(false);

        private ReactiveProperty<Vector3> _target = new();
        public void Bind(TowerViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            _viewModel = viewModel;
            missile.gameObject.SetActive(false);

            //Доп.защита на случай убийства моба другой башней
            viewModel.Targets.ObserveRemove().Subscribe(_ =>
            {
                if (viewModel.Targets.Count == 0)
                {
                    Debug.Log("Доп.защита на случай убийства моба другой башней");
                    missile.gameObject.SetActive(false);
                }
            }).AddTo(ref d);
            _disposable = d.Build();
        }

        public void Fire(ReactiveProperty<Vector3> position)
        {
            _target = position;
            
            Direction(position.CurrentValue); //Поворот зоны выстрела мгновенный 
            missile.transform.localPosition = Vector3.zero; //Размещение снаряда в точке 0
            //TODO Переделать
            var particle = fire.GetComponent<ParticleSystem>();
            if (particle != null) particle.Play(); //Запуск эффекта выстрела
            
            //  Debug.Log(_viewModel.TowerEntityId + " Начало выстрела");
        }

        private void Direction(Vector3 toDirection)
        {
            var fromDirection = new Vector3(_viewModel.Position.Value.x, 0, _viewModel.Position.Value.y);
            toDirection.y = 0;
            var direction = toDirection - fromDirection;
            transform.rotation = Quaternion.LookRotation(direction);
        }
        public ReactiveProperty<bool> Missile()
        {
            missile.gameObject.SetActive(true);
            _isMoving.OnNext(true);
            Debug.Log(_viewModel.TowerEntityId + " Выстрел");
            return _isMoving;
            //TODO Выстрел
        }
        
        public void Explosion()
        {
//            Debug.Log(_viewModel.TowerEntityId + " Снаряд долетел ВЗРЫВ");
            //Взрыв в _target
            explosion.position = new Vector3(_target.CurrentValue.x, explosion.position.y, _target.CurrentValue.z);
            StartCoroutine(StartExplosion());
            //TODO Передать в сервис дорог координаты попадания, для показа шейдера мапинг (растрескивание)
        }

        private IEnumerator StartExplosion()
        {
            explosion.gameObject.SetActive(true);
            var particle = explosion.GetComponent<ParticleSystem>();
            if (particle == null) yield break;
            var playing = true;
            //TODO Передать в сервис дорог координаты попадания, для показа шейдера мапинг (растрескивание)
            particle.Play();
            while (playing)
            {
                if (!particle.isPlaying)
                {
                    explosion.gameObject.SetActive(false);
                    playing = false;
                }
                yield return null;
            }
            
            
             //Запуск эффекта выстрела
            
        }

        private void Update()
        {
         /*   if (_viewModel.GameSpeed.CurrentValue == 0) return;
            if (!_isMoving.CurrentValue) return;
            
            var speedEntity = _viewModel.GameSpeed.CurrentValue * _viewModel.SpeedShot * AppConstants.SHOT_BASE_SPEED;
            //TODO Расчет координат полета от типа снаряда
            missile.position = Vector3.MoveTowards(missile.position, _target.CurrentValue,  Time.deltaTime * speedEntity);
            var toDirection = _target.CurrentValue - new Vector3(_viewModel.Position.CurrentValue.x, 0, _viewModel.Position.CurrentValue.y);
            var fromDirection = new Vector3(0, 0, 1f);
            Debug.Log(_viewModel.TowerEntityId + " Снаряд летит, скорость = " + speedEntity);
            missile.rotation = Quaternion.FromToRotation(fromDirection, toDirection);
            // Debug.Log("Позиция Выстрела " + ShotUniqueId + " " + Position.CurrentValue);
            if (Vector3.Distance(missile.position, _target.CurrentValue) < 0.1)
            {
                //Выстрел достиг цели (моба) движение прекращаем, останавливается /заканчивается
                _isMoving.OnNext(false);
            }
                */
            //Движение и поворот снаряда
            //Долетел до _target
            //
        }
        
        private void OnDestroy()
        {
            _disposable?.Dispose();
        }

        public IEnumerator StartMissile()
        {
            missile.gameObject.SetActive(true);
            _isMoving.OnNext(true);
//            Debug.Log(_viewModel.TowerEntityId + " Выстрел");
            yield return null;
            Debug.DrawLine(missile.transform.position, _target.CurrentValue);
            while (_isMoving.Value)
            {
                

                var speedEntity = _viewModel.GameSpeed.CurrentValue * _viewModel.SpeedShot * AppConstants.SHOT_BASE_SPEED;
                yield return null;
                yield return new WaitUntil(() => speedEntity != 0); //На паузе не стреляем
                //TODO Расчет координат полета от типа снаряда
                
                missile.transform.position = Vector3.MoveTowards(missile.transform.position, _target.CurrentValue,  Time.deltaTime * speedEntity);
                var toDirection = _target.CurrentValue - new Vector3(transform.position.x, 0, transform.position.y);
//                Debug.Log(_viewModel.TowerEntityId + " Снаряд летит, скорость = " + speedEntity);
               // var toDirection = _shotEntity.FinishPosition.Value - _shotEntity.StartPosition;
                var fromDirection = new Vector3(0, 0, 1f);

                
                //missile.transform.rotation = Quaternion.FromToRotation(fromDirection, toDirection);
                // Debug.Log("Позиция Выстрела " + ShotUniqueId + " " + Position.CurrentValue);
                if (Vector3.Distance(missile.transform.position, _target.CurrentValue) < 0.1)
                {
                    //Выстрел достиг цели (моба) движение прекращаем, останавливается /заканчивается
                    missile.gameObject.SetActive(false);
                    _isMoving.OnNext(false);
                }

                if (_target == null || _target.Value == null)
                {
                    missile.gameObject.SetActive(false);
                    _isMoving.OnNext(false);
                }

                yield return null;
            }

            //yield return new WaitUntil(() => !_isMoving.CurrentValue);
            // Debug.Log("Выстрел достиг цели " + ShotUniqueId);
        }
    }
}