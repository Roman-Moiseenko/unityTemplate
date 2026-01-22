using System;
using System.Collections;
using Game.Common;
using Game.GamePlay.View.Mobs;
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
        [SerializeField] protected ShotBinder shotBinder;
        
        private IDisposable _disposable;
        protected TowerViewModel _viewModel;
        protected readonly ReactiveProperty<bool> _isMoving = new(false);

        protected ReactiveProperty<Vector3> _target = new();
        public void Bind(TowerViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            _viewModel = viewModel;
            shotBinder.Bind(viewModel);

            _disposable = d.Build();
        }

        public virtual void FirePrepare(MobViewModel mobViewModel)
        {
            _target = mobViewModel.PositionTarget;
            Direction(_target.CurrentValue); //Поворот зоны выстрела мгновенный 
            shotBinder.FirePrepare(mobViewModel);

            //TODO Переделать
            var particle = fire.GetComponent<ParticleSystem>();
            if (particle != null) particle.Play(); //Запуск эффекта выстрела

        }
        public virtual void FireStart()
        {
            StartCoroutine(shotBinder.FireStart());
        }

        /**
         * Эффект после выстрела
         */
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
            shotBinder.StopShot();
        }
        
    }
}