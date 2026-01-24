using System;
using System.Collections;
using Game.GamePlay.View.Mobs;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Towers
{
    public class TowerShotBinder : MonoBehaviour
    {
        [SerializeField] protected Transform fire;
        [SerializeField] protected Transform missile;
        [SerializeField] protected Transform explosion;
        [SerializeField] protected ShotBinder shotBinder;

        protected IDisposable _disposable;
        protected TowerViewModel _viewModel;
        
        public bool IsFree;
        protected MobViewModel MobTarget;
        protected Coroutine mainCoroutine;

        protected ReactiveProperty<Vector3> _targetPosition = new();

        public virtual void Bind(TowerViewModel viewModel)
        {
            IsFree = true;

            var d = Disposable.CreateBuilder();
            _viewModel = viewModel;
            shotBinder.Bind(viewModel);

            _disposable = d.Build();
        }

        public virtual void FireToTarget(MobViewModel mobViewModel)
        {
            if (mainCoroutine != null) StopCoroutine(mainCoroutine);

            IsFree = false;
            MobTarget = mobViewModel;
            _targetPosition = mobViewModel.PositionTarget;
            Direction(_targetPosition.CurrentValue); //Поворот зоны выстрела мгновенный 

            var particle = fire.GetComponent<ParticleSystem>();
            if (particle != null) particle.Play(); //Запуск эффекта выстрела

            shotBinder.FirePrepare(mobViewModel);
            mainCoroutine = StartCoroutine(StartShotFire());
        }

        protected virtual IEnumerator StartShotFire()
        {
            yield return shotBinder.FireStart();

            yield return StartExplosion();
            IsFree = true;
        }
        

        private void Direction(Vector3 toDirection)
        {
            var fromDirection = new Vector3(_viewModel.Position.Value.x, 0, _viewModel.Position.Value.y);
            toDirection.y = 0;
            var direction = toDirection - fromDirection;
            transform.rotation = Quaternion.LookRotation(direction);
        }

        /**
         * Эффект после выстрела
         */
        private IEnumerator StartExplosion()
        {
            var particle = explosion.GetComponent<ParticleSystem>();
            if (particle == null) yield break;

            explosion.gameObject.SetActive(true);
            explosion.position = new Vector3(_targetPosition.CurrentValue.x, explosion.position.y,
                _targetPosition.CurrentValue.z);
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

        public virtual void StopShot()
        {
            shotBinder?.StopShot();
        }
    }
}