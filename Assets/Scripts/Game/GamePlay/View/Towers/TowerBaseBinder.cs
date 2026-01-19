using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Common;
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

        private IDisposable _disposable;
        private readonly Dictionary<MobEntity, IDisposable> _targetsDisposables = new();

        private TowerViewModel _viewModel;
        private TowerBinder _towerBinder;
        private TowerShotBinder _towerShotBinder;
        private Sequence Sequence { get; set; }
        
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
                        .OnComplete(() =>
                        {
                            Sequence.Kill();
                        });
                }
                else
                {
                    CreateTower();
                }
            }).AddTo(ref d);
            CreateShot();

            viewModel.Targets.ObserveAdd().Subscribe(e =>
            {
                var mobEntity = e.Value;

                var disposableTarget = this.FromCoroutine(() => TowerFire(mobEntity))
                    .Subscribe(onNext: _ => { },
                        onErrorResume: _ =>
                        {
                            _viewModel.RemoveTarget(mobEntity);
                            _towerShotBinder.StopShot();
                        },
                        onCompleted: _ => { });

                _targetsDisposables.TryAdd(mobEntity, disposableTarget);
            }).AddTo(ref d);

            viewModel.Targets.ObserveRemove().Subscribe(e =>
            {
                var mobEntity = e.Value;
                _targetsDisposables[mobEntity].Dispose();
            }).AddTo(ref d);

            _disposable = d.Build();
        }

        private IEnumerator TowerFire(MobEntity mobEntity)
        {
            //Поворачиваем башню
            _towerBinder.StartDirection(mobEntity.Position.CurrentValue);
            //Подготовка выстрела
            _towerShotBinder.FirePrepare(mobEntity);
            _towerBinder.FireAnimation(); // Анимация выстрела
            //Запускаем снаряд и ждем когда долетит
            yield return _towerShotBinder.FireStart();
            _towerShotBinder.FireFinish();
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

          /**
           * Перезапуск атаки после обновления башни
           */
          private void RestartAttack()
          {
              foreach (var mobEntity in _viewModel.Targets.ToList())
              {
                  _viewModel.RemoveTarget(mobEntity);
              }
              _towerShotBinder.StopShot();
              _viewModel.TowerEntity.FreeToFire();
          }      
        
        private void OnDestroy()
        {
            if (Sequence.IsActive())
            {
                Sequence.Kill();
                Sequence = null;
            }
            _disposable?.Dispose();
        }
        


    }
}