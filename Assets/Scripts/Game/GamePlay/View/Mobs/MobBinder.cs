using System;
using Cysharp.Threading.Tasks;
using Game.Common;
using Game.State.Maps.Mobs;
using ObservableCollections;
using R3;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.GamePlay.View.Mobs
{
    public class MobBinder : MonoBehaviour
    {
        [SerializeField] private Transform _healthBar;
        [SerializeField] private MobVisibleBinder mobVisible;

        MaterialPropertyBlock matBlock;
        MeshRenderer meshRenderer;
        public MobViewModel ViewModel;
        private Vector3 _targetPosition;
        private HealthBar _healthBarBinder;

        private Quaternion _targetDirection;

        private float _mobY;
        private int _currentIndexListPoint;

        public int UnityId;

        IDisposable disposable;
        public ReactiveProperty<bool> Free = new(true); //Доступность в пуле

        public void Bind(MobViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            Free.Value = false;

            ViewModel = viewModel;
            UnityId = viewModel.UniqueId;
            _mobY = ViewModel.IsFly ? 0.9f : 0.0f;
            

            _healthBarBinder = _healthBar.GetComponent<HealthBar>();
            _healthBarBinder.Bind(
                ViewModel.CameraService.Camera,
                ViewModel.MaxHealth,
                ViewModel.CurrentHealth,
                ViewModel.Level
            );
            _currentIndexListPoint = 0;
            mobVisible.Bind(viewModel);


            //Вращаем в движении

            viewModel.IsMoving.Subscribe(v =>
            {
                //Debug.Log(v);
            }).AddTo(ref d);
            viewModel.IsAttack.Subscribe().AddTo(ref d);
            viewModel.AnimationDelete.Where(v => v == true).Subscribe(_ =>
            {
                //TODO Анимация удаления объекта После окончания:
                viewModel.FinishCurrentAnimation.Value = true;
            }).AddTo(ref d);

            viewModel.State.Subscribe(newState =>
            {
                //TODO Переключаем анимацию от состояния моба.
                if (newState == MobState.Attacking)
                {
                    //   Debug.Log("Моб " + viewModel.MobEntityId + " Аттакует");
                }
            }).AddTo(ref d);
            
            gameObject.SetActive(false);
            viewModel.StartGo.Where(x => x).Subscribe(_ =>
            {
                transform.position = new Vector3(viewModel.StartPosition.x, _mobY, viewModel.StartPosition.y);
                //TODO Включение анимации или эффекта длп старта
                //Начальная позиция - координата первой дороги от портала
                _targetPosition = viewModel.GetTargetPosition(_currentIndexListPoint);

                
                //поворачиваем модель
                transform.rotation = Quaternion.LookRotation(new Vector3(viewModel.StartDirection.x, 0,
                    viewModel.StartDirection.y));
                gameObject.SetActive(true);
            }).AddTo(ref d);
            
            //TODO Проверить 
            //При проигрывании анимации удаляем все подписки, чтоб не сработали, т.е. сущность уже удалена
            viewModel.AnimationDelete.Where(x => x).Subscribe(_ => disposable.Dispose()).AddTo(ref d);
            
            disposable = d.Build();


            
            //gameObject.SetActive(true);
        }

        public void Update()
        {
            if (ViewModel.IsMoving.Value)
            {
                if (_targetPosition == transform.position) //Дошли то след.точки
                {
                    var newValue = ViewModel.RoadPoints[_currentIndexListPoint].Direction;

                    var direction = new Vector3(newValue.x, ViewModel.IsFly ? 0.9f : 0.0f, newValue.y);
                    transform.rotation = Quaternion.LookRotation(direction);
                    //Направление поворота Проверяем, поменялось ли направление

                    _currentIndexListPoint++;
                    if (_currentIndexListPoint == ViewModel.RoadPoints.Count) ViewModel.IsMoving.OnNext(false);

                    _targetPosition = ViewModel.GetTargetPosition(_currentIndexListPoint);
                }

                var speedMob = AppConstants.MOB_BASE_SPEED * ViewModel.GetSpeedMob();

                transform.position = Vector3.MoveTowards(
                    transform.position,
                    _targetPosition,
                    Time.deltaTime * speedMob);
                //TODO Временное решение
                ViewModel.Position.OnNext(new Vector2(transform.position.x, transform.position.z));
            }
        }

        public void LateUpdate()
        {
            if (Free.Value) return;
            _healthBarBinder.OnUpdate();
        }

        private void OnDestroy()
        {
            disposable.Dispose();
        }

        public void FreeUp()
        {
            gameObject.SetActive(false);
            Free.OnNext(true);
            disposable.Dispose();
        }

        //Для учета попадания по мобу
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Shot"))
            {
                //TODO Находим Данные от Выстрела и наносим уронм мобу через viewModel.DamageService.SetDamage()
            }
        }
    }
}