using System;
using System.Collections;
using System.Collections.Generic;
using Game.State.Maps.Mobs;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Castle
{
    public class CastleBinder : MonoBehaviour
    {
        [SerializeField] private CastleShotsBinder shots;
        private IDisposable _disposable;
        private CastleViewModel _viewModel;
        //private readonly Dictionary<MobEntity, IDisposable> _targetsDisposables = new();
        private Coroutine _coroutine;
        public void Bind(CastleViewModel viewModel)
        {
            _viewModel = viewModel;
            var d = Disposable.CreateBuilder();
            transform.position = new Vector3(
                viewModel.Position.x,
                0,
                viewModel.Position.y
            );
            
            shots.Bind(viewModel);
            viewModel.Target
                .ObserveAdd()
                .Subscribe(e =>
                {
                    var mobEntity = e.Value;
                    _coroutine = StartCoroutine(CastleFire(mobEntity));
                })
                .AddTo(ref d);
            
            viewModel.Target.ObserveRemove().Subscribe(e =>
            {
                //var mobEntity = e.Value;
                StopCoroutine(_coroutine);
                shots.StopShot();
            }).AddTo(ref d);
            _disposable = d.Build();
        }
        
        
        private IEnumerator CastleFire(MobEntity mobEntity)
        {
            //Подготовка выстрела
            shots.FirePrepare(mobEntity);
            FireAnimation(); // Анимация выстрела
            //Запускаем снаряд и ждем когда долетит
            yield return shots.FireStart();
            shots.FireFinish();
            _viewModel.CastleEntity.RemoveTarget(mobEntity);
        }
        
        private void OnDestroy()
        {
            _disposable?.Dispose();
        }

        private void FireAnimation()
        {
            //TODO Включаем анимацию выстрела из 3х башен
//            Debug.Log("FireAnimation Castle");
        }
    }
}