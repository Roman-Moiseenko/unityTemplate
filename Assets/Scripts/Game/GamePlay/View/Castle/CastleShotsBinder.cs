using System;
using System.Collections;
using DG.Tweening;
using Game.State.Maps.Mobs;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Castle
{
    public class CastleShotsBinder : MonoBehaviour
    {
        [SerializeField] private CastleShotBinder shotLeft;
        [SerializeField] private CastleShotBinder shotCenter;
        [SerializeField] private CastleShotBinder shotRight;
        
        private CastleViewModel _viewModel;
        private readonly Vector3 _positionLeft = new (0f, 0.5f, -1f);
        private readonly Vector3 _positionCenter = new (0f, 0.90f, 0f);
        private readonly Vector3 _positionRight = new (0f, 0.5f, 1f);
        private Sequence _sequence;

        private ReactiveProperty<Vector3> _target = new();
        
        public void Bind(CastleViewModel viewModel)
        {
            _viewModel = viewModel;
            shotLeft.Bind(_positionLeft);
            shotCenter.Bind(_positionCenter);
            shotRight.Bind(_positionRight);
        }

        public void StartShot(MobEntity mobEntity)
        {
            Debug.Log("StartShot Castle");
            _viewModel.CastleEntity.RemoveTarget(mobEntity);
        }



        public void FirePrepare(MobEntity mobEntity)
        {
            _target = mobEntity.PositionTarget;
            shotLeft.FirePrepare(mobEntity);
            shotCenter.FirePrepare(mobEntity);
            shotRight.FirePrepare(mobEntity);
        }
        
        public IEnumerator FireStart()
        {
            var duration = _viewModel.CastleEntity.Speed;
            //Запуск полета снарядов
            if (_viewModel.GameSpeed.CurrentValue != 0) duration /= _viewModel.GameSpeed.CurrentValue;
            
            shotLeft.FireStart(duration);
            shotCenter.FireStart(duration);
            shotRight.FireStart(duration);
            yield return new WaitForSeconds(duration);
        }
        
        public void FireFinish()
        {
            StopShot();
            /*shotLeft.FireFinish();
            shotCenter.FireFinish();
            shotRight.FireFinish();*/
            StartCoroutine(StartExplosion());
            //Эффект взрыва на дороге
        }
        
        private IEnumerator StartExplosion()
        {
           /* explosion.gameObject.SetActive(true);
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
            }*/
            yield return null;
        }

        public void StopShot()
        {
            shotLeft.FireFinish();
            shotCenter.FireFinish();
            shotRight.FireFinish();
        }
    }
}