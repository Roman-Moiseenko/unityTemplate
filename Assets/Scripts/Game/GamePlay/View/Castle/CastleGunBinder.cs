using System;
using DG.Tweening;
using Game.GamePlay.View.Mobs;
using Game.State.Maps.Mobs;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Castle
{
    public class CastleGunBinder : MonoBehaviour
    {
        [SerializeField] private Transform turret;
        [SerializeField] private Transform barrel;
        [SerializeField] private ParticleSystem fireEffect;
        [SerializeField] private CastleShotBinder shot;
        public ReactiveProperty<bool> IsShotComplete => shot.IsShotComplete;
        private Sequence Sequence { get; set; }

        public void Bind(CastleViewModel viewModel)
        {
            shot.Bind(viewModel);
            //Запуск полета снарядов
        }
        
        public void Fire(MobViewModel mobViewModel)
        {
            shot.FirePrepare(mobViewModel);
            var direction = mobViewModel.PositionTarget.CurrentValue - turret.position;
            var targetRotation = Quaternion.LookRotation(direction);
            
            Sequence = DOTween.Sequence();
            Sequence
                .SetDelay(0.1f)
                .Append(
                    turret
                        .DORotateQuaternion(targetRotation, 0.1f)
                        )
                .Append(
                    barrel
                        .DOPunchPosition(new Vector3(0, 0, -0.1f), 0.1f, 1, 1)
                        .From(false)) // 1. Откат орудия
                .AppendCallback(() =>
                {
                    // 2. Пламя
                    if (fireEffect != null) fireEffect.Play(); //Запуск эффекта выстрела
                    // 3. Полет снаряда
                    shot.Fire();
                })
                .OnComplete(() =>
                {
                    barrel.localPosition = Vector3.zero;
                    Sequence.Kill();
                });
            

        }

        public void StopFire()
        {
            KillSequence();
            shot.FireFinish();
        }

        private void OnDestroy()
        {
            KillSequence();
        }
        
        private void KillSequence()
        { 
            if (Sequence.IsActive())
            {
                Sequence.Kill();
                Sequence = null;
            }
        }
    }
}