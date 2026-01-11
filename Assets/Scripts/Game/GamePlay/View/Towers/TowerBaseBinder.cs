using System;
using System.Collections;
using Game.State.Maps.Mobs;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Towers
{
    public class TowerBaseBinder : MonoBehaviour
    {
        [SerializeField] private Transform container;
        [SerializeField] private Animator animator;

        [SerializeField] private ParticleSystem finish;
        [SerializeField] private ParticleSystem start;
        [SerializeField] private Transform shot;
        
        private IDisposable _disposable;
        private TowerViewModel _viewModel;

        private TowerBinder _towerBinder;
        private TowerShotBinder _towerShotBinder;

        public void Bind(TowerViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            _viewModel = viewModel;
            
            transform.position = new Vector3(
                viewModel.Position.CurrentValue.x,
                transform.position.y,
                viewModel.Position.CurrentValue.y
            );

            _viewModel.Level.Skip(1).Subscribe(_ =>
            {
                //TODO Запускаем анимацию шейдеров и частиц
                start.Play();
                finish.Play();
            });
            
            _viewModel.NumberModel.Subscribe(number =>
            {
                //Если Префаб уже был, то запускаем анимацию
                if (_towerBinder != null)
                {
                    animator.Play("tower_level_up");
                }
                else
                {
                    CreateTower();
                } 
            }).AddTo(ref d);
            CreateShot();
            
            //TODO подписка на выстрел от towerEntity
            viewModel.Targets.ObserveAdd().Subscribe(e =>
            {
                var mobEntity = e.Value;
//                Debug.Log(viewModel.TowerEntityId + " Выстрел по мобу " + mobEntity.UniqueId);
                StartCoroutine(TowerFire(mobEntity));
                
                //Сначала поворачиваем башню
             /*   _towerBinder.Direction(mobEntity.Position.CurrentValue)
                    .Skip(1)
                    .Where(x => !x)
                    .Subscribe(_ =>
                    {
                        Debug.Log("FIRE " + mobEntity.UniqueId);
                        //Поворачиваем снаряд, запуск эффекта выстрела
                        _towerShotBinder.Fire(mobEntity.PositionTarget);
                        _towerBinder.FireAnimation(); // Анимация выстрела
                        //Запускаем снаряд и ждем когда долетит
                        _towerShotBinder.Missile()
                            .Skip(1).Where(x => !x)
                            .Subscribe(_ =>
                            {
                                Debug.Log("Missile Subscribe " + mobEntity.UniqueId);
                                _towerShotBinder.Explosion();
                                viewModel.RemoveTarget(mobEntity);
                            });
                    });
                */
              /*  viewModel.Targets.ObserveRemove().Subscribe(v =>
                {
                    if (mobEntity == v.Value)
                    {
                        Debug.Log("Dispose " + mobEntity.UniqueId);
                        dd.Dispose();                        
                    }
                });
                */
            }).AddTo(ref d);
            //viewModel.Targets.
            
            //TODO Загружаем данные о выстреле
            //TODO Префаб выстрела, содержит модель снаряда, Эффект выстрела, Эффект взрыва
            /*
            viewModel.Direction.Skip(1).Subscribe(newValue =>
            {
                if (shot != null) //Вращение префаба выстрела, мгновенное без анимации 
                {
                    var fromDirection = new Vector3(viewModel.Position.Value.x, 0, viewModel.Position.Value.y);
                    var toDirection = new Vector3(newValue.x, 0, newValue.y);
                    var direction = toDirection - fromDirection;
                    shot.transform.rotation = Quaternion.LookRotation(direction);
                    
                    //_isDirection = true;
                }
            }).AddTo(ref d);
            
            */
            //Binder выстрела содержит команды
            //Shot - показать эффект выстрела
            //Explode - показать эффект взрыва
            
            _disposable = d.Build();
        }


        private IEnumerator TowerFire(MobEntity mobEntity)
        {
            //Поворачиваем башню
            yield return _towerBinder.StartDirection(mobEntity.Position.CurrentValue);
            
            //Подготовка выстрела
//            Debug.Log("FIRE " + mobEntity.UniqueId);
            _towerShotBinder.Fire(mobEntity.PositionTarget);
            _towerBinder.FireAnimation(); // Анимация выстрела
            
            //Запускаем снаряд и ждем когда долетит
            yield return _towerShotBinder.StartMissile();
//            Debug.Log("Missile Finish " + mobEntity.UniqueId);
            _towerShotBinder.Explosion();
            _viewModel.RemoveTarget(mobEntity);
            
        }
        
        private void DestroyTower()
        {
            Destroy(_towerBinder.gameObject);
            _towerBinder = null;
        }
        
        private void CreateTower()
        {
            var towerNumber = _viewModel.NumberModel;
            var towerType = _viewModel.ConfigId;

            var prefabTowerLevelPath =
                $"Prefabs/Gameplay/Towers/{towerType}/{towerType}-{towerNumber}"; 

            var towerPrefab = Resources.Load<TowerBinder>(prefabTowerLevelPath);
            _towerBinder = Instantiate(towerPrefab, container.transform);
            _towerBinder.Bind(_viewModel);
        }

        private void CreateShot()
        {
            var towerType = _viewModel.ConfigId;
            var prefabTowerShotPath =
                $"Prefabs/Gameplay/Towers/{towerType}/Shot-{towerType}";
            var shotPrefab = Resources.Load<TowerShotBinder>(prefabTowerShotPath);
            _towerShotBinder = Instantiate(shotPrefab, shot.transform);
            _towerShotBinder.Bind(_viewModel);
            
        }
        
        private void OnDestroy()
        {
            _disposable?.Dispose();
        }

        public void AnimationChangeModel()
        {
            DestroyTower();
            CreateTower();
        }

        public void AnimationFinish()
        {
            //Debug.Log("Конец анимации");
        }

    }
}