using System;
using System.Collections;
using DG.Tweening;
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
        private ReactiveProperty<Vector3> _targetPosition = new();

        private IDisposable _disposable;
        public WarriorViewModel ViewModel;
        
        public int UniqueId => ViewModel.UniqueId;
        
        public void Bind(WarriorViewModel viewModel)
        {
            //Подготавливаем все данные для модели
            ViewModel = viewModel;
            attackBinder.Bind(viewModel);
            healthBarBinder.Bind(
                viewModel.MaxHealth,
                viewModel.CurrentHealth,
                0
            );
            transform.position = viewModel.Position.CurrentValue;
            
            var d = Disposable.CreateBuilder();
            //Анимация, движение и атака от состояния
            viewModel.FsmWarrior.Fsm.StateCurrent.Subscribe(state =>
            {
                if (state.GetType() == typeof(FsmWarriorNew))
                {
                    Debug.Log("FsmWarriorNew " + viewModel.UniqueId);
                }

                //Идем к точке спавна, включить анимацию движения
                if (state.GetType() == typeof(FsmWarriorGoToPlacement))
                {
                    Debug.Log("FsmWarriorToPlacement " + viewModel.UniqueId);

                    _targetPosition.Value = viewModel.Placement;
                    transform.rotation = Quaternion.LookRotation(_targetPosition.Value - transform.position);
                }

                if (state.GetType() == typeof(FsmWarriorAwait)) //Повернуться к дороге и вкл.анимацию ожидания
                {
                    Debug.Log("FsmWarriorAwait " + ViewModel.UniqueId);
                    var direction = viewModel.AvailablePath[0].Direction;
                    transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y));
                }

                if (state.GetType() == typeof(FsmWarriorGoToMob))
                {
                    Debug.Log("FsmWarriorGoToMob " + viewModel.UniqueId);
                    var target = ViewModel.FsmWarrior.GetTarget();

                    //TODO Заменить на движение по AvailablePath
                    _targetPosition = target.PositionTarget;
                }

                if (state.GetType() == typeof(FsmWarriorAttack))
                {
                    Debug.Log("FsmWarriorAttack " + viewModel.UniqueId);
                    StartCoroutine(FireUpdateWarrior());
                }

                if (state.GetType() == typeof(FsmWarriorGoToRepair))
                {
                    Debug.Log("FsmWarriorGoToRepair " + viewModel.UniqueId);
                    _targetPosition.Value = new Vector3(ViewModel.StartPosition.CurrentValue.x, 0,
                        ViewModel.StartPosition.CurrentValue.y);
                }

                if (state.GetType() == typeof(FsmWarriorRepair))
                {
                    Debug.Log("FsmWarriorRepair " + viewModel.UniqueId);
                }

                if (state.GetType() == typeof(FsmWarriorDead))
                {
                    Debug.Log("FsmWarriorDead " + viewModel.UniqueId);
                }
            }).AddTo(ref d);
            _disposable = d.Build();

            //Запускаем warrior
            ViewModel.FsmWarrior.Fsm.SetState<FsmWarriorGoToPlacement>();
        }

        private IEnumerator FireUpdateWarrior()
        {
            fire.Play();
            ViewModel.SetDamageAfterShot(); //Наносим урон, без отображения полета пули
            yield return new WaitForSeconds(ViewModel.Speed);
            if (ViewModel.MobTarget.CurrentValue != null) StartCoroutine(FireUpdateWarrior());
        }

        public void LateUpdate()
        {
            healthBarBinder.OnUpdate();
        }

        private void Update()
        {
            if (!ViewModel.FsmWarrior.IsMoving) return;
            
            transform.position =
                Vector3.MoveTowards(transform.position, _targetPosition.Value, 1.3f * Time.deltaTime);
            if (Vector3.Distance(transform.position, _targetPosition.Value) < 0.02f)
                ViewModel.IsMovingFinish(); //Нет коллайдера определения, переключаем вручную
            
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