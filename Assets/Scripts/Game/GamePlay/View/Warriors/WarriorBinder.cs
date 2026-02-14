using System;
using System.Collections;
using DG.Tweening;
using Game.Common;
using Game.GamePlay.Fsm.WarriorStates;
using Game.GamePlay.View.Mobs;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Warriors
{
    public class WarriorBinder : MonoBehaviour
    {
        [SerializeField] private HealthBar healthBarBinder;
        [SerializeField] private ParticleSystem fire;
        [SerializeField] private WarriorAttackBinder attackBinder;
        
        //[SerializeField] private WarriorAttackBinder attackBinder;
        private Vector3 _targetPosition;

        private IDisposable _disposable;
        public WarriorViewModel ViewModel;
        
        public int UniqueId => ViewModel.UniqueId;
        
        public void Bind(WarriorViewModel viewModel)
        {
            //Подготавливаем все данные для модели
            ViewModel = viewModel;
          //  attackBinder.Bind(viewModel);
            healthBarBinder.Bind(
                viewModel.MaxHealth.CurrentValue,
                viewModel.CurrentHealth,
                0
            );
            attackBinder.Bind(viewModel);
            
            transform.position = viewModel.Position.CurrentValue;
            
            var d = Disposable.CreateBuilder();
            //Анимация, движение и атака от состояния
            viewModel.FsmWarrior.Fsm.StateCurrent.Subscribe(state =>
            {
                //Идем к точке спавна, включить анимацию движения
                if (state.GetType() == typeof(FsmWarriorGoToPlacement))
                {
                    _targetPosition = viewModel.FsmWarrior.GetPosition();
                    var direction = _targetPosition - transform.position;
                    if (direction != Vector3.zero) transform.rotation = Quaternion.LookRotation(direction);
                }
                //Повернуться к дороге и вкл.анимацию ожидания
                if (state.GetType() == typeof(FsmWarriorAwait)) 
                {
                    var direction = viewModel.AvailablePath.Direction;
                    transform.rotation = Quaternion.LookRotation(direction);
                }

                if (state.GetType() == typeof(FsmWarriorGoToMob))
                {
                    _targetPosition = viewModel.FsmWarrior.GetPosition();
                    //Debug.Log("FsmWarriorGoToMob " + _targetPosition);
                    var direction = _targetPosition - transform.position;
                    if (direction != Vector3.zero) transform.rotation = Quaternion.LookRotation(direction);    
                }

                if (state.GetType() == typeof(FsmWarriorAttack))
                {
                    _targetPosition = viewModel.FsmWarrior.GetPosition();
                    _targetPosition.y = 0;
                    var direction = _targetPosition - transform.position;
                    //Debug.Log("diretion "  + direction + " " + _targetPosition + " = " + transform.position);
                    transform.rotation = Quaternion.LookRotation(direction);
                    StartCoroutine(FireUpdateWarrior());
                }

                if (state.GetType() == typeof(FsmWarriorGoToRepair))
                {
                    _targetPosition = MyFunc.Vector2To3(ViewModel.StartPosition.CurrentValue);
                    transform.rotation = Quaternion.LookRotation(_targetPosition - transform.position);
                }

            }).AddTo(ref d);
            _disposable = d.Build();

            //Запускаем warrior
            ViewModel.FsmWarrior.Fsm.SetState<FsmWarriorGoToPlacement>(viewModel.Placement);
        }

        private IEnumerator FireUpdateWarrior()
        {
            fire.Play();
            ViewModel.SetDamageAfterShot(); //Наносим урон, без отображения полета пули
            yield return new WaitForSeconds(ViewModel.Speed);
            if (ViewModel.MobTarget.CurrentValue != null)
            {
                StartCoroutine(FireUpdateWarrior());
            }
        }

        public void LateUpdate()
        {
            healthBarBinder.OnUpdate();
        }

        private void Update()
        {
            
            if (!ViewModel.FsmWarrior.IsMoving) return;
          /*  if (_targetPosition.y != 0)
            {
                Debug.Log(ViewModel.FsmWarrior.Fsm.StateCurrent.CurrentValue.GetType());
            }
            */
         //   Debug.Log("transform.position = " + transform.position + " _targetPosition=" + _targetPosition);
            transform.position =
                Vector3.MoveTowards(transform.position, _targetPosition, 1.3f * Time.deltaTime);
            ViewModel.Position.OnNext(transform.position);
            if (Vector3.Distance(transform.position, _targetPosition) < 0.02f)
            {
                transform.position = _targetPosition;
                ViewModel.Position.OnNext(transform.position);
                ViewModel.IsMovingFinish(transform.position); //Нет коллайдера определения, переключаем вручную
            }
        }

        private void OnDestroy()
        {
            ViewModel.Dispose();
            _disposable?.Dispose();
        }

        public ReactiveProperty<bool> StartDeadAnimation()
        {
            var result = new ReactiveProperty<bool>(false);
            transform.DOScale(Vector3.zero, 0.2f)
                .From(Vector3.one)
                .OnComplete(() => { result.OnNext(true); });
            //StartCoroutine(AnimationDead(result));
            return result;
        }

        private IEnumerator AnimationDead(ReactiveProperty<bool> result)
        {
            //TODO Запускаем анимацию по заврешению
            yield return new WaitForSecondsRealtime(0.2f);
            result.OnNext(true);
        }
    }
}