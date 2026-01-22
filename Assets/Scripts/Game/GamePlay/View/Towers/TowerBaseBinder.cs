using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Common;
using Game.GamePlay.View.Mobs;
using Game.State.Maps.Mobs;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Towers
{
    public class TowerBaseBinder : MonoBehaviour
    {
        [SerializeField] private Transform container;
        [SerializeField] private ParticleSystem finish;
        [SerializeField] private ParticleSystem start;
        [SerializeField] private Transform shot;
        [SerializeField] private TowerVisibleBinder visibleBinder;

        private IDisposable _disposable;
        private Coroutine _mainCoroutine;
        private TowerViewModel _viewModel;
        private TowerBinder _towerBinder;
        private TowerShotBinder _towerShotBinder;
        private Sequence Sequence { get; set; }

        private ReactiveProperty<Vector3> _firsTarget;

        public void Bind(TowerViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            _viewModel = viewModel;

            //Для башен с точкой размещения солдат не подключаем Коллайдер Видимости
            //Также сделать для Бафных башен ??
            if (viewModel.TowerEntity.IsPlacement)
            {
                _mainCoroutine = StartCoroutine(PlacementUpdateTower());
            }
            else
            {
                visibleBinder.Bind(viewModel);
                CreateShot();
                _mainCoroutine = StartCoroutine(FireUpdateTower());
            }

            transform.position = new Vector3(
                viewModel.Position.CurrentValue.x,
                transform.position.y,
                viewModel.Position.CurrentValue.y
            );

            _viewModel.Level.Skip(1).Subscribe(_ =>
            {
                //Запускаем анимацию шейдеров и частиц при обновлении уровня башни
                start.Play();
                finish.Play();
                RestartAttack();
            }).AddTo(ref d);

            _viewModel.NumberModel.Subscribe(number =>
            {
                //Если Префаб уже был, то запускаем анимацию
                if (_towerBinder != null)
                {
                    Sequence = DOTween.Sequence();
                    Sequence
                        .Append(
                            container
                                .DOScale(Vector3.zero, 0.5f)
                                .From(Vector3.one)
                                .SetEase(Ease.OutCubic))
                        .AppendCallback(() =>
                        {
                            DestroyTower();
                            CreateTower();
                        })
                        .Append(
                            container.transform
                                .DOScale(Vector3.one, 0.5f)
                                .SetEase(Ease.InCubic))
                        .OnComplete(() => { Sequence.Kill(); });
                }
                else
                {
                    CreateTower();
                }
            }).AddTo(ref d);
            _disposable = d.Build();
        }

        private IEnumerator PlacementUpdateTower()
        {
            while (true)
            {
                _viewModel.AddWarriorsTower();
                yield return new WaitForSeconds(6f);
            }
        }

        private IEnumerator FireUpdateTower()
        {
            while (true)
            {
                //Обходим все цели в модели
                foreach (var (uniqueId, mobViewModel) in _viewModel.MobTargets.ToList())
                {
                    if (!mobViewModel.IsDead.CurrentValue)
                    {
                        //Если цель жива, запускаем процесс атаки
                        yield return _towerBinder.StartDirection(mobViewModel.Position.CurrentValue);
                        FireOneTarget(mobViewModel);
                    }
                    else
                    {
                        //Если Цель мертва, удаляем из списка целей 
                        _viewModel.RemoveTarget(mobViewModel);
                    }
                }

                yield return new WaitForSeconds(_viewModel.Speed);
            }
        }

        private void FireOneTarget(MobViewModel mobViewModel)
        {
            //Подготовка выстрела
            _towerShotBinder.FirePrepare(mobViewModel);
            _towerBinder.FireAnimation(); // Анимация выстрела
            //Запускаем снаряд и ждем когда долетит
            _towerShotBinder.FireStart();
            _towerShotBinder.FireFinish();
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


        /**
         * Перезапуск атаки после обновления башни
         */
        private void RestartAttack()
        {
            foreach (var mobEntity in _viewModel.Targets.ToList())
            {
                _viewModel.RemoveTargetEntity(mobEntity);
            }

            _towerShotBinder.StopShot();
            _viewModel.TowerEntity.FreeToFire();
        }

        private void OnDestroy()
        {
            StopCoroutine(_mainCoroutine);
            if (Sequence.IsActive())
            {
                Sequence.Kill();
                Sequence = null;
            }
            _disposable?.Dispose();
        }
    }
}