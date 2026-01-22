using System;
using System.Collections;
using Game.GamePlay.View.Mobs;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Warriors
{
    public class WarriorBinder : MonoBehaviour
    {
        private Vector3 _targetPosition;
        private bool _isMoving;

        private IDisposable _disposable;
        private Coroutine _mainCoroutine;
        private WarriorViewModel _viewModel;
        private Coroutine _coroutine;
        private float _speedMove;
        public int UniqueId => _viewModel.UniqueId;
        public void Bind(WarriorViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            _viewModel = viewModel;
            _speedMove = viewModel.Speed;
            //Debug.Log("Warrior binded " + viewModel.UniqueId + " " + viewModel.StartPosition + " => " + viewModel.PlacementPosition);
            
            transform.position = viewModel.StartPosition;

            //TODO Как в Tower
            
            
            if (viewModel.StartPosition != viewModel.PlacementPosition)
            {
                _targetPosition = viewModel.PlacementPosition;
                _isMoving = true;
            }
        /*    
            viewModel.MobTarget.Subscribe(mobViewModel =>
            {
                if (mobViewModel == null)
                { //На случай, если моба убьет не Замок или цель вышла из зоны поражения
                    StopFire();
                    return;
                }
                _coroutine = StartCoroutine(FireOneTarget(mobViewModel));
                //TODO Протестировать и придумать отписку после удаления моба
                mobViewModel.IsDead
                    .Where(x => x)
                    .Subscribe(_ => _viewModel.RemoveTarget(mobViewModel));
            }).AddTo(ref d);
            */
            _disposable = d.Build();
            _mainCoroutine = StartCoroutine(FireUpdateWarrior());
        }

        private IEnumerator FireUpdateWarrior()
        {
            while (true)
            {
                if (_viewModel.MobTarget.CurrentValue != null)
                {
                    if (_viewModel.MobTarget.CurrentValue.IsDead.CurrentValue)
                    {
                        _viewModel.RemoveTarget(_viewModel.MobTarget.CurrentValue);
                    }
                    else
                    {
                        yield return FireOneTarget(_viewModel.MobTarget.CurrentValue);
                    }
                }
                yield return null;
            }
        }

        
        
        private IEnumerator FireOneTarget(MobViewModel mobViewModel)
        {
           // while (!mobViewModel.IsDead.CurrentValue)
           // {
                if (_viewModel.IsDead.CurrentValue) yield break;
                
                _viewModel.SetDamageAfterShot(); //Без отображения полета пули
                yield return new WaitForSeconds(_viewModel.Speed);
          //  }
        }

        private void StopFire()
        {
            //if(_coroutine != null) StopCoroutine(_coroutine);
        }

        private void Update()
        {
            if (_isMoving)
            {
                transform.position = Vector3.MoveTowards(transform.position, _targetPosition, 1.3f * Time.deltaTime);
                if (Vector3.Distance(transform.position, _targetPosition) < 0.02f)
                {
                    _isMoving = false;
                }
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!other.gameObject.CompareTag("MobVisible")) return;
            if (_viewModel.MobTarget.CurrentValue == null) //Если цели нет, назначаем первую вошедшую
            {
                var mobBinder = other.gameObject.GetComponent<MobVisibleBinder>();
                if (mobBinder.ViewModel.IsDead.CurrentValue) return;
                _viewModel.SetTarget(mobBinder.ViewModel);
            }
        }
        
        private void OnCollisionStay(Collision other)
        {
            if (!other.gameObject.CompareTag("MobVisible")) return;
            if (_viewModel?.MobTarget.CurrentValue == null) //Если цели нет, назначаем первую в области поражения
            {
                var mobBinder = other.gameObject.GetComponent<MobBinder>();
                if (mobBinder == null) return;
                if (mobBinder.ViewModel == null) return;
                
                if (mobBinder.ViewModel.IsDead.CurrentValue) return;
                _viewModel.SetTarget(mobBinder?.ViewModel);
            }
        }
        
        private void OnCollisionExit(Collision other)
        {
            if (!other.gameObject.CompareTag("MobVisible")) return;
            if (_viewModel.MobTarget.CurrentValue != null)
            {
                var mobBinder = other.gameObject.GetComponent<MobBinder>();
                _viewModel.RemoveTarget(mobBinder.ViewModel);
            }
        }

        private void OnDestroy()
        {
            StopCoroutine(_mainCoroutine);
            if (_coroutine != null) StopCoroutine(_coroutine);
        }
    }
}