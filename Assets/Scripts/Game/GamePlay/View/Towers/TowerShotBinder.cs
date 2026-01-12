using System;
using System.Collections;
using Game.Common;
using Game.State.Maps.Mobs;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Towers
{
    public class TowerShotBinder : MonoBehaviour
    {
        [SerializeField] protected Transform fire;
        [SerializeField] protected Transform missile;
        [SerializeField] protected Transform explosion;
        
        private IDisposable _disposable;
        protected TowerViewModel _viewModel;
        protected readonly ReactiveProperty<bool> _isMoving = new(false);
        protected float _baseYMissile = 0f;

        protected ReactiveProperty<Vector3> _target = new();
        public void Bind(TowerViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            _viewModel = viewModel;
            missile.gameObject.SetActive(false);
            _baseYMissile = missile.position.y;
            
            //Доп.защита на случай убийства моба другой башней
            viewModel.Targets.ObserveRemove().Subscribe(_ =>
            {
                if (viewModel.Targets.Count == 0)
                {
//                    Debug.Log("Доп.защита на случай убийства моба другой башней");
                    missile.gameObject.SetActive(false);
                }
            }).AddTo(ref d);
            _disposable = d.Build();
        }

        public virtual void FirePrepare(MobEntity mobEntity)
        {
            _target = mobEntity.PositionTarget;
            
            Direction(_target.CurrentValue); //Поворот зоны выстрела мгновенный 
            missile.transform.localPosition = new Vector3(0, _baseYMissile, 0); //Размещение снаряда в точке 0
            //TODO Переделать
            var particle = fire.GetComponent<ParticleSystem>();
            if (particle != null) particle.Play(); //Запуск эффекта выстрела
            
            //  Debug.Log(_viewModel.TowerEntityId + " Начало выстрела");
        }
        public virtual IEnumerator FireStart()
        {
            missile.gameObject.SetActive(true);
            _isMoving.OnNext(true);
//            Debug.Log(_viewModel.TowerEntityId + " Выстрел");
            yield return null;
        //    Debug.DrawLine(missile.transform.position, _target.CurrentValue);
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
                    StopShot();
                    _isMoving.OnNext(false);
                }

                if (_target == null || _target.Value == null)
                {
                    StopShot();
                    _isMoving.OnNext(false);
                }

                yield return null;
            }

            //yield return new WaitUntil(() => !_isMoving.CurrentValue);
            // Debug.Log("Выстрел достиг цели " + ShotUniqueId);
        }

        
        public virtual void FireFinish()
        {
            explosion.position = new Vector3(_target.CurrentValue.x, explosion.position.y, _target.CurrentValue.z);
            StartCoroutine(StartExplosion());
            //TODO Передать в сервис дорог координаты попадания, для показа шейдера мапинг (растрескивание)
        }

        private void Direction(Vector3 toDirection)
        {
            var fromDirection = new Vector3(_viewModel.Position.Value.x, 0, _viewModel.Position.Value.y);
            toDirection.y = 0;
            var direction = toDirection - fromDirection;
            transform.rotation = Quaternion.LookRotation(direction);
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
        }

        
        private void OnDestroy()
        {
            _disposable?.Dispose();
        }



        public void StopShot()
        {
            missile.gameObject.SetActive(false);
        }
    }
}