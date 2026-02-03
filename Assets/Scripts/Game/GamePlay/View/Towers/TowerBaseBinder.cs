using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Common;
using Game.GamePlay.View.Mobs;
using Game.State.Maps.Mobs;
using ObservableCollections;
using R3;
using UnityEngine;
using UnityEngine.VFX;

namespace Game.GamePlay.View.Towers
{
    /**
     * Основой Binder башни, связывает все действия
     */
    public class TowerBaseBinder : MonoBehaviour
    {
        [SerializeField] private Transform container;
        [SerializeField] private VisualEffect after;
        [SerializeField] private VisualEffect befor;
        [SerializeField] private Transform shot;
        [SerializeField] private Transform areaAction;
        [SerializeField] private TowerVisibleBinder visibleBinder;
        [SerializeField] private TowerUnVisibleBinder unvisibleBinder;

        private List<TowerShotBinder> _shotBinders = new();
        private IDisposable _disposable;
        private Coroutine _mainCoroutine;
        private TowerViewModel _viewModel;
        private TowerBinder _towerBinder;
        private TowerShotBinder _towerShotBinder;
        private Sequence Sequence { get; set; }

        private ReactiveProperty<Vector3> _firsTarget;

        private AreaBinder _areaBinder;

        private void OnEnable()
        {
            after.gameObject.SetActive(true);
            befor.gameObject.SetActive(true);
            after.Stop();
            befor.Stop();
        }

        public void Bind(TowerViewModel viewModel)
        {
            _viewModel = viewModel;
            transform.position = new Vector3(
                viewModel.Position.CurrentValue.x,
                transform.position.y,
                viewModel.Position.CurrentValue.y
            );

            CreateTower();
            CreateArea();

            //Для башен с точкой размещения солдат не подключаем Коллайдер Видимости
            //Также сделать для Бафных башен ??
            if (viewModel.IsPlacement)
            {
                _viewModel.AddWarriorsTower();
                _mainCoroutine = StartCoroutine(PlacementUpdateTower());
            }
            else
            {
                visibleBinder.Bind(viewModel); //Подключаем коллайдер видимости
                if (viewModel.MinDistance > 0)
                    unvisibleBinder.Bind(viewModel); //Подключаем коллайдер зоны недоступности

                CreateShot(); //для ускорения сразу создаем 1 снаряд в пул
                _mainCoroutine = StartCoroutine(FireUpdateTower());
            }

            // ПОДПИСКИ //
            var d = Disposable.CreateBuilder();

            //Если есть площадь, то подписываемся на события
            if (_areaBinder != null)
            {
                _viewModel.ShowArea.Subscribe(show =>
                {
                    if (show)
                    {
                        _areaBinder.Show(_viewModel.GetAreaRadius());
                    }
                    else
                    {
                        _areaBinder.Hide();
                    }
                }).AddTo(ref d);
            }


            //Запуск эффекта обновления уровня
            _viewModel.Level.Skip(1).Subscribe(_ =>
            {
                StartCoroutine(EffectsUpgradeTower());
                RestartAttack();
            }).AddTo(ref d);
            //Смена модели при обновлении на четных уровнях
            _viewModel.NumberModel.Skip(1).Subscribe(number =>
            {
                //Если Префаб уже был, то запускаем анимацию
                Sequence = DOTween.Sequence();
                Sequence
                    .Append(
                        container
                            .DOScale(Vector3.zero, 0.5f)
                            .From(Vector3.one)
                            .SetEase(Ease.OutCubic).SetUpdate(true))
                    .AppendCallback(() =>
                    {
                        DestroyTower();
                        CreateTower();
                    })
                    .Append(
                        container.transform
                            .DOScale(Vector3.one, 0.5f)
                            .SetEase(Ease.InCubic).SetUpdate(true))
                    .OnComplete(() => { Sequence.Kill(); }).SetUpdate(true);
            }).AddTo(ref d);
            _disposable = d.Build();
        }

        private IEnumerator EffectsUpgradeTower()
        {
            //_viewModel.FinishEffectLevelUp.Value = false;
            befor.playRate = 1.5f;
            after.playRate = 1.5f;
            befor.Play();
            yield return new WaitForSecondsRealtime(0.5f);
            befor.Stop();
            after.Play();
            yield return new WaitForSecondsRealtime(0.5f);
            after.Stop();
            _viewModel.FinishEffectLevelUp.OnNext(true);
        }

        private IEnumerator PlacementUpdateTower()
        {
            while (true)
            {
                if (_viewModel.IsDeadAllWarriors())
                {
                    //TODO Ускорение при быстром вызове волны
                    yield return new WaitForSeconds(10f);
                    _viewModel.AddWarriorsTower();
                }

                yield return null;
            }
        }

        private IEnumerator FireUpdateTower()
        {
            while (true)
            {
                yield return null;
                //Обходим все цели в модели
                foreach (var (uniqueId, mobViewModel) in _viewModel.MobTargets.ToList())
                {
                    if (!mobViewModel.IsDead.CurrentValue)
                    {
                        //Если цель жива, запускаем процесс атаки
                        if (!_viewModel.IsMultiShot) //Для одиночного выстрела поворачиваем башню
                            yield return _towerBinder.StartDirection(mobViewModel.Position.CurrentValue);
                        _towerBinder.FireAnimation(); // Анимация выстрела
                        FindFreeShot().FireToTarget(mobViewModel);
                    }
                    else
                    {
                        //Если Цель мертва, удаляем из списка целей 
                        // _viewModel.RemoveTarget(mobViewModel);
                    }
                }

                yield return new WaitForSeconds(_viewModel.Speed);
            }
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

        private void CreateArea()
        {
            //Нет области Атаки
            if (_viewModel.GetAreaRadius() == Vector3.zero) return;

            var prefabAreaPath = _viewModel.IsPlacement
                ? "Prefabs/Gameplay/Towers/Area/AreaPlacement"
                : "Prefabs/Gameplay/Towers/Area/AreaAttack";

            var areaPrefab = Resources.Load<AreaBinder>(prefabAreaPath);
            _areaBinder = Instantiate(areaPrefab, areaAction.transform);
            _areaBinder.Bind();
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
            var towerType = _viewModel.ConfigId;
            var prefabTowerShotPath =
                $"Prefabs/Gameplay/Towers/{towerType}/Shot-{towerType}";
            var shotPrefab = Resources.Load<TowerShotBinder>(prefabTowerShotPath);
            var towerShotBinder = Instantiate(shotPrefab, shot.transform);
            towerShotBinder.Bind(_viewModel);
            _shotBinders.Add(towerShotBinder);
            return towerShotBinder;
        }


        /**
         * Перезапуск атаки после обновления башни
         */
        private void RestartAttack()
        {
            foreach (var shotBinder in _shotBinders)
            {
                shotBinder.StopShot();
            }
        }

        private void OnDestroy()
        {
            StopCoroutine(_mainCoroutine);
            if (Sequence.IsActive())
            {
                Sequence.Kill();
                Sequence = null;
            }

            Destroy(visibleBinder.gameObject);
            Destroy(visibleBinder);


            Destroy(unvisibleBinder.gameObject);
            Destroy(unvisibleBinder);


            Destroy(_towerBinder.gameObject);
            Destroy(_towerBinder);

            if (_towerShotBinder != null)
            {
                Destroy(_towerShotBinder.gameObject);
                Destroy(_towerShotBinder);
            }

            foreach (var shotBinder in _shotBinders)
            {
                Destroy(shotBinder.gameObject);
                Destroy(shotBinder);
            }


            _disposable?.Dispose();
        }
    }
}