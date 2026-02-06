using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.GamePlay.Fsm.WarriorStates;
using Game.GamePlay.View.Mobs;
using MVVM.Storage;
using R3;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.GamePlay.View.Warriors
{
    public class WarriorBinder : MonoBehaviour
    {
        [SerializeField] private HealthBar healthBarBinder;

        //[SerializeField] private WarriorVisibleBinder visibleBinder;
        [SerializeField] private WarriorAttackBinder attackBinder;
        private ReactiveProperty<Vector3> _targetPosition = new();
        private bool _isMoving;

        private IDisposable _disposable;
        private Coroutine _mainCoroutine;
        public WarriorViewModel ViewModel;
        private Coroutine _coroutine;
        private float _speedMove;
        private bool _isAttack;


        public int UniqueId => ViewModel.UniqueId;


        public void Bind(WarriorViewModel viewModel)
        {
            //Подготавливаем все данные для модели

            ViewModel = viewModel;
            //visibleBinder.Bind(viewModel);
            attackBinder.Bind(viewModel);
            healthBarBinder.Bind(
                viewModel.MaxHealth,
                viewModel.CurrentHealth,
                0
            );

            _speedMove = viewModel.Speed;
            transform.position = viewModel.Position.CurrentValue;

            //Debug.Log("Warrior binded " + viewModel.UniqueId + " " + viewModel.StartPosition + " => " + viewModel.PlacementPosition);

            var d = Disposable.CreateBuilder();
            //Анимация, движение и атака от состояния
            viewModel.FsmWarrior.Fsm.StateCurrent.Subscribe(state =>
            {
                //Идем к точке спавна, включить анимацию движения
                if (state.GetType() == typeof(FsmWarriorToPlacement))
                {
                    //var position = viewModel.AvailablePath[0].Point; //Первая позиция в списке, это Placement
                    //Debug.Log(viewModel.Placement);
                    _targetPosition.Value = viewModel.Placement; //new Vector3(position.x, 0, position.y);
                    //Повернуться в сторону Placement
                    transform.rotation = Quaternion.LookRotation(_targetPosition.Value - transform.position);
                    _isMoving = true;
                    _isAttack = false;
                }

                if (state.GetType() == typeof(FsmWarriorAwait)) //Повернуться к дороге и вкл.анимацию ожидания
                {
                    Debug.Log("FsmWarriorAwait " + ViewModel.UniqueId);
                    var direction = viewModel.AvailablePath[0].Direction;
                    transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y));
                    _isMoving = false;
                }

                if (state.GetType() == typeof(FsmWarriorGoToMob))
                {
                    var target = ViewModel.FsmWarrior.GetTarget();
                    Debug.Log($"{ViewModel.UniqueId} Начали движение к мобу {target.UniqueId}");
                    //TODO Заменить на движение по AvailablePath
                    _targetPosition = target.PositionTarget;
                    _isMoving = true;
                    _isAttack = false;
                    //Получить модель моба через Fsm
                    //Идти к мобу 
                }

                if (state.GetType() == typeof(FsmWarriorAttack))
                {
                    var target = ViewModel.FsmWarrior.GetTarget();

                    Debug.Log($"{ViewModel.UniqueId} Начинаем Атаку по Мобу {target.UniqueId}");
                    _isMoving = false;
                    _isAttack = true;
                  //  StartCoroutine(FireUpdateWarrior());
                    //Получить модель моба через Fsm
                    //Атакуем моба
                }

                if (state.GetType() == typeof(FsmWarriorGoToRepair))
                {
                    Debug.Log("Идем восстанавливаться к башне " + ViewModel.UniqueId);
                    _targetPosition.Value = new Vector3(ViewModel.StartPosition.CurrentValue.x, 0,
                        ViewModel.StartPosition.CurrentValue.y);
                    _isMoving = true;
                    //Идем к башне
                }

                if (state.GetType() == typeof(FsmWarriorRepair))
                {
                    _isMoving = false;
                }

                if (state.GetType() == typeof(FsmWarriorDead))
                {
                    _isMoving = false;
                    _isAttack = false;
                    //Включаем анимацию смерти
                }
            }).AddTo(ref d);

            _disposable = d.Build();

            //Запускаем warrior
            ViewModel.FsmWarrior.Fsm.SetState<FsmWarriorToPlacement>();
        }

        private IEnumerator FireUpdateWarrior()
        {
           // while (ViewModel.MobTarget.CurrentValue != null)
           // {
                ViewModel.SetDamageAfterShot(); //Без отображения полета пули
                yield return new WaitForSeconds(ViewModel.Speed);
          //  }
        }

        public void LateUpdate()
        {
            healthBarBinder.OnUpdate();
        }

        private void Update()
        {
            if (_isMoving)
            {
//                if (ViewModel.FsmWarrior.IsGoToMob()) Debug.Log($" {ViewModel.UniqueId} Идем к " + _targetPosition.CurrentValue);

                transform.position =
                    Vector3.MoveTowards(transform.position, _targetPosition.Value, 1.3f * Time.deltaTime);
                if (Vector3.Distance(transform.position, _targetPosition.Value) < 0.02f)
                {
                    _isMoving = false; //Авто выключение
                    //Нет коллайдера определения, переключаем вручную
                    if (ViewModel.FsmWarrior.IsPlacement()) ViewModel.FsmWarrior.Fsm.SetState<FsmWarriorAwait>();
                    if (ViewModel.FsmWarrior.IsGoToRepair()) ViewModel.FsmWarrior.Fsm.SetState<FsmWarriorRepair>();
                }
            }
        }


        private void OnDestroy()
        {
            //StopCoroutine(_mainCoroutine);
            //if (_coroutine != null) StopCoroutine(_coroutine);
/*
            foreach (var (key, disposable) in ViewModel.MobPullDisposables.ToList())
            {
                disposable?.Dispose();
                ViewModel.MobDisposables.Remove(key);
            }

            foreach (var (key, disposable) in ViewModel.MobDisposables.ToList())
            {
                disposable?.Dispose();
                ViewModel.MobDisposables.Remove(key);
            }      
*/
            
            ViewModel.Dispose();
            _disposable?.Dispose();
        }

        public ReactiveProperty<bool> StartDeadAnimation()
        {
            var result = new ReactiveProperty<bool>(false);
            StartCoroutine(AnimationDead(result));
            //result.OnNext(true);
            return result;
        }

        private IEnumerator AnimationDead(ReactiveProperty<bool> result)
        {
            //TODO Запускаем анимацию по заврешению
            yield return new WaitForSecondsRealtime(0.3f);
            result.OnNext(true);
        }
    }
}