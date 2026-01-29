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
        [SerializeField] protected ShotBinder shotBinder;
        [SerializeField] protected ExplosionBinder explosionBinder;

        protected IDisposable _disposable;
        protected TowerViewModel _viewModel;
        
        public bool IsFree;
        protected MobViewModel MobTarget;
        protected Coroutine mainCoroutine;

        protected ReactiveProperty<Vector3> _targetPosition = new();

        private void Awake()
        {
            if (explosionBinder != null) explosionBinder.Stop(); 
            
        }

        public virtual void Bind(TowerViewModel viewModel)
        {
            IsFree = true;

            var d = Disposable.CreateBuilder();
            _viewModel = viewModel;
            shotBinder.Bind(viewModel);
            if (explosionBinder != null) explosionBinder.Bind();
            
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
            if (explosionBinder != null) yield return StartExplosion();    
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
          explosionBinder.Play(new Vector3(_targetPosition.CurrentValue.x, 0.1f,
              _targetPosition.CurrentValue.z));
          yield return new WaitForSeconds(0.3f);
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