using System;
using System.Collections;
using Game.GamePlay.View.Mobs;
using R3;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.GamePlay.View.Warriors
{
    public class WarriorBinder : MonoBehaviour
    {
        
        [SerializeField] private HealthBar _healthBarBinder;
        private Vector3 _targetPosition;
        private bool _isMoving;

        private IDisposable _disposable;
        private Coroutine _mainCoroutine;
        private WarriorViewModel _viewModel;
        private Coroutine _coroutine;
        private float _speedMove;

        private CharacterController _controller;
        public float stopDistance = 0.2f; // Расстояние, на котором остановимся
        private const float Speed = 3.0f; // Скорость движения воина

        //   private float rotationSpeed = 2.0f; // Скорость поворота
        private const float AvoidanceForce = 1.0f; // Сила отталкивания от других воинов
        private const float AvoidanceRadius = 0.20f; // Радиус обнаружения других воинов
        private const float FinalOffsetFromTarget = 0.1f; // Дополнительный отступ при завершении движения
        private bool _arrivedAtInitialTarget = false;
        private float _initialYPosition = 0f;

        public int UniqueId => _viewModel.UniqueId;

        void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }

        public void Bind(WarriorViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            _viewModel = viewModel;
            _speedMove = viewModel.Speed;
            //Debug.Log("Warrior binded " + viewModel.UniqueId + " " + viewModel.StartPosition + " => " + viewModel.PlacementPosition);

            transform.position = viewModel.StartPosition;
            _initialYPosition = viewModel.StartPosition.y;

            //TODO Как в Tower

            if (viewModel.StartPosition != viewModel.PlacementPosition)
            {
                _targetPosition = viewModel.PlacementPosition;
                _isMoving = true;
            }
            _healthBarBinder.Bind(
                viewModel.MaxHealth,
                viewModel.CurrentHealth,
                0
            );

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
                yield return null;
                if (_viewModel.MobTarget.CurrentValue != null)
                {
                    _viewModel.SetDamageAfterShot(); //Без отображения полета пули
                    yield return new WaitForSeconds(_viewModel.Speed);
                }
            }
        }
        
        public void LateUpdate()
        {
            _healthBarBinder.OnUpdate();
        }
        private void Update()
        {
            if (!_isMoving) return;

            if (_arrivedAtInitialTarget)
            {
                // Здесь можно добавить логику для окончательной расстановки или бездействия
                if (transform.position.y != _initialYPosition)
                {
                    transform.position = new Vector3(transform.position.x, _initialYPosition, transform.position.z);
                }

                _arrivedAtInitialTarget = false;
                return;
            }

            Vector3 currentPositionXZ = new Vector3(transform.position.x, _initialYPosition, transform.position.z);
            Vector3 targetPositionXZ = new Vector3(_targetPosition.x, _initialYPosition, _targetPosition.z);
            Vector3 direction = (targetPositionXZ - currentPositionXZ).normalized;
            direction.y = 0; // Игнорируем компоненту Y для направления
            Vector3 desiredVelocity = direction * Speed;
            // Избегание столкновений с другими воинами
            Vector3 avoidanceVelocity = CalculateAvoidance();
            avoidanceVelocity.y = 0; // Игнорируем компоненту Y для избегания
            desiredVelocity += avoidanceVelocity;
            // Применяем движение
            // CharacterController.Move автоматически обрабатывает гравитацию, если она нужна.
            // Если воин не должен падать, убедитесь, что гравитация в CharacterController
            // не приводит к нежелательным эффектам, или компенсируйте ее.
            // Для чисто XZ движения, мы просто применяем горизонтальную составляющую.
            Vector3 finalMovement = new Vector3(desiredVelocity.x, 0, desiredVelocity.z);
            _controller.Move(finalMovement * Time.deltaTime);
            // Чтобы воин оставался на своей начальной Y-позиции, принудительно устанавливаем ее.
            // CharacterController.Move может вызывать небольшие смещения по Y из-за лестниц или неровностей.
            if (transform.position.y != _initialYPosition)
            {
                transform.position = new Vector3(transform.position.x, _initialYPosition, transform.position.z);
            }

            // Поворот в сторону движения (или к целевой точке)
            /*     if (finalMovement.magnitude > 0.1f) // Чтобы не крутиться на месте
                 {
                     Quaternion targetRotation = Quaternion.LookRotation(finalMovement.normalized);
                     transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                 }
                 */
            // Проверка достижения целевой точки
            // Используем расстояние только по XZ для проверки
            if (Vector3.Distance(currentPositionXZ, targetPositionXZ) < stopDistance)
            {
                FinishingMovement();
                _arrivedAtInitialTarget = true;
            }


            /*

            if (_isMoving)
            {
                transform.position = Vector3.MoveTowards(transform.position, _targetPosition, 1.3f * Time.deltaTime);
                if (Vector3.Distance(transform.position, _targetPosition) < 0.02f)
                {
                    _isMoving = false;
                }
            }
            */
        }

        private Vector3 CalculateAvoidance()
        {
            Vector3 avoidanceVector = Vector3.zero;
            // Используем OverlapSphere, но затем игнорируем Y-компоненту при расчете вектора отталкивания
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, AvoidanceRadius);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject != gameObject &&
                    hitCollider.CompareTag("Warrior")) // Убедитесь, что у воинов есть тег "Warrior"
                {
                    Vector3 awayFromWarrior = (transform.position - hitCollider.transform.position);
                    awayFromWarrior.y = 0; // Игнорируем Y-компоненту
                    awayFromWarrior.Normalize();
                    avoidanceVector += awayFromWarrior * AvoidanceForce;
                }
            }

            return avoidanceVector;
        }

        void FinishingMovement()
        {
            // После достижения основной целевой точки, делаем небольшой "отход", чтобы избежать наложения
            // При этом также сохраняем Y-координату.
            var finalDirection = (transform.position - _targetPosition);
            finalDirection.y = 0; // Игнорируем Y при расчете финального направления
            finalDirection.Normalize();
            var finalPosition = _targetPosition + finalDirection * FinalOffsetFromTarget;
            finalPosition.y = _initialYPosition; // Убеждаемся, что финальная позиция по Y соответствует начальной
            // Можно использовать CharacterController.Move для плавного доезда или просто установить позицию
            // controller.Move((finalPosition - transform.position) * Time.deltaTime * speed); // Если нужен плавный доезд
            transform.position = finalPosition; // Или просто ставим в финальную позицию
            Debug.Log(gameObject.name + " достиг своей целевой точки и отошел для финальной расстановки.");
            _isMoving = false;
        }

        void OnDrawGizmosSelected()
        {
            // Для визуализации радиуса избегания в редакторе
            if (Application.isPlaying)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, AvoidanceRadius);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("MobVisible")) return;
            var mobBinder = other.gameObject.GetComponent<MobVisibleBinder>();
            _viewModel.PullTargets.Add(mobBinder.ViewModel);

        }
        

        private void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.CompareTag("MobVisible")) return;
            if (_viewModel.PullTargets.Count != 0)
            {
                //Когда моб выходит из зоны видимости, удаляем из Пула
                var mobBinder = other.gameObject.GetComponent<MobBinder>();
                if (mobBinder.ViewModel.IsDead.CurrentValue) return; //Лаг задержки удаления модели
                _viewModel.PullTargets.Remove(mobBinder.ViewModel);
            }
            
        }

        private void OnDestroy()
        {
            StopCoroutine(_mainCoroutine);
            if (_coroutine != null) StopCoroutine(_coroutine);
        }
    }
}