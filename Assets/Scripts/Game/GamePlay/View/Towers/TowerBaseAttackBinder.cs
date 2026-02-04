using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.GamePlay.View.Towers
{
    public class TowerBaseAttackBinder : TowerBaseBinder<TowerAttackViewModel>
    {
        
        [SerializeField] protected Transform shot;
        [SerializeField] protected TowerVisibleBinder visibleBinder;
        [SerializeField] protected TowerUnVisibleBinder unvisibleBinder;

        private readonly List<TowerShotBinder> _shotBinders = new();
        
        protected override void OnBind(TowerAttackViewModel viewModel)
        {
            
            visibleBinder.Bind(viewModel); //Подключаем коллайдер видимости
            if (viewModel.MinDistance > 0)
                unvisibleBinder.Bind(viewModel); //Подключаем коллайдер зоны недоступности
            MainCoroutine = StartCoroutine(FireUpdateTower());
            CreateShot(); //для ускорения сразу создаем 1 снаряд в пул
        }
        
        
        private IEnumerator FireUpdateTower()
        {
            while (true)
            {
                yield return null;
                //Обходим все цели в модели
                foreach (var (uniqueId, mobViewModel) in ((TowerAttackViewModel)ViewModel).MobTargets.ToList())
                {
                    if (!mobViewModel.IsDead.CurrentValue)
                    {
                        //Если цель жива, запускаем процесс атаки
                        if (!ViewModel.IsMultiShot) //Для одиночного выстрела поворачиваем башню
                            yield return TowerBinder.StartDirection(mobViewModel.Position.CurrentValue);
                        TowerBinder.FireAnimation(); // Анимация выстрела
                        FindFreeShot().FireToTarget(mobViewModel);
                    }
                    else
                    {
                        //Если Цель мертва, удаляем из списка целей 
                        // _viewModel.RemoveTarget(mobViewModel);
                    }
                }

                yield return new WaitForSeconds(ViewModel.Speed);
            }
        }
        
        
        private TowerShotBinder FindFreeShot()
        {
            foreach (var shotBinder in _shotBinders)
                if (shotBinder.IsFree)
                    return shotBinder;

            return CreateShot();
        }

        private TowerShotBinder CreateShot()
        {
            var towerType = ViewModel.ConfigId;
            var prefabTowerShotPath =
                $"Prefabs/Gameplay/Towers/{towerType}/Shot-{towerType}";
            var shotPrefab = Resources.Load<TowerShotBinder>(prefabTowerShotPath);
            var towerShotBinder = Instantiate(shotPrefab, shot.transform);
            towerShotBinder.Bind((TowerAttackViewModel)ViewModel);
            _shotBinders.Add(towerShotBinder);
            return towerShotBinder;
        }
        
        protected override void RestartAfterUpdate()
        {
            foreach (var shotBinder in _shotBinders)
            {
                shotBinder.StopShot();
            }
        }
        
        private void OnDestroy()
        {
            StopCoroutine(MainCoroutine);
            
            Destroy(visibleBinder.gameObject);
            Destroy(visibleBinder);
            Destroy(unvisibleBinder.gameObject);
            Destroy(unvisibleBinder);
            foreach (var shotBinder in _shotBinders)
            {
                Destroy(shotBinder.gameObject);
                Destroy(shotBinder);
            }


          //  _disposable?.Dispose();
        }
        
    }
}