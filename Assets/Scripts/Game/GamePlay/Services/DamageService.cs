using System;
using System.Collections;
using System.Collections.Generic;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.States;
using Game.GamePlay.View.Mobs;
using Game.GamePlay.View.Towers;
using Game.Settings.Gameplay.Entities.Tower;
using Game.State.Maps.Castle;
using Game.State.Maps.Shots;
using Game.State.Maps.Towers;
using Game.State.Root;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using Scripts.Utils;
using Unity.VisualScripting;
using UnityEngine;


namespace Game.GamePlay.Services
{
    public class DamageService
    {
        private readonly FsmGameplay _fsmGameplay;
        private readonly GameplayStateProxy _gameplayState;
        private readonly TowersSettings _towersSettings;
        private readonly WaveService _waveService;
        private readonly TowersService _towersService;
        private readonly ShotService _shotService;
        private readonly Coroutines _coroutines;
        private readonly RewardProgressService _rewardProgressService;
        public ObservableList<DamageEntity> AllDamages = new();

        //private CastleEntity _castleEntity;
        // private 

        public DamageService(
            FsmGameplay fsmGameplay,
            GameplayStateProxy gameplayState,
            TowersSettings towersSettings,
            WaveService waveService,
            TowersService towersService,
            ShotService shotService,
            RewardProgressService rewardProgressService
        )
        {
            _fsmGameplay = fsmGameplay;
            _gameplayState = gameplayState;
            _towersSettings = towersSettings;
            _waveService = waveService;
            _towersService = towersService;
            _shotService = shotService;
            _rewardProgressService = rewardProgressService;
            _coroutines = GameObject.Find("[COROUTINES]").GetComponent<Coroutines>();

            // AllTowers = _gameplayState.;

            waveService.AllMobsMap.ObserveAdd().Subscribe(e =>
            {
                var mobEntity = e.Value.Value;
                mobEntity.Position.Subscribe(newPosition => { });

//                Debug.Log(mobEntity.UniqueId + " " + mobEntity.Health.CurrentValue + " " + mobEntity.IsDead.Value);
                //Проверяем, что моб мертв выдаем награды и другое 
                mobEntity.IsDead.Skip(1).Subscribe(
                    v =>
                    {
                        if (v)
                        {
                            rewardProgressService.RewardKillMob(mobEntity.RewardCurrency, mobEntity.Position.CurrentValue);
                            
                            waveService.AllMobsMap.Remove(e.Value.Key);
                        }
                    }
                );
            });

            fsmGameplay.Fsm.StateCurrent.Subscribe(e =>
            {
                if (e.GetType() == typeof(FsmStateGamePlay))
                {
                }
            });

            _shotService.Shots.ObserveRemove().Subscribe(e =>
            {
                var shot = e.Value;
                if (_waveService.AllMobsMap.TryGetValue(shot.MobEntityId, out var mobEntity))
                {
                    if (!shot.Single)
                    {
                        //TODO урон по области, то ищем остальных мобов в радиусе 0.5f
                    }
                    else
                    {
                       // mobEntity.SetDamage(shot.Damage);
                        var damage = new DamageEntity
                        {
                            Position = mobEntity.Position.CurrentValue,
                            Damage = Mathf.FloorToInt(mobEntity.SetDamage(shot.Damage)),
                            Type = shot.DamageType,
                        };

                        AllDamages.Add(damage); //Добавить в список 
                        if (shot.Debuff != null)
                        {
                            mobEntity.SetDebuff(shot.ConfigId, shot.Debuff);
                        }
                    }
                }
                else
                {
                    throw new Exception($"Моб не найден {shot.MobEntityId}");
                }
            });
        }

        public void Update()
        {
            // ObservableSystem.DefaultFrameProvider  = new NewThreadSleepFrameProvider();
            //   Observable.EveryUpdate(UnityFrameProvider.Update).Subscribe(v =>
            //  {

            if (_fsmGameplay.IsGamePause.Value) return;
            if (_waveService.AllMobsMap.Count == 0) return;

            foreach (var towerEntity in _gameplayState.Towers)
            {
                if (towerEntity.IsShot.Value) continue;
                //Если башня не стреляет, запускаем корутин
                towerEntity.IsShot.Value = true;
                _coroutines.StartCoroutine(TowerFireShot(towerEntity));
            }

            //TODO Стрельба крепости
            if (!_gameplayState.Castle.IsShot.Value)
            {
                _gameplayState.Castle.IsShot.Value = true;
                _coroutines.StartCoroutine(CastleFireShot());
            }

            //  });
        }

        public IEnumerator CastleFireShot()
        {
            yield return new WaitUntil(() => !_fsmGameplay.IsGamePause.Value); //На паузе не стреляем

            var castleTowerOnePosition = new Vector2Int(0, -1);
            var castleTowerTwoPosition = new Vector2Int(0, 1);
            var castleIsShooting = false;
            //Обходим список мобов
            foreach (var keyMobEntity in _waveService.AllMobsMap)
            {
                var mobEntity = keyMobEntity.Value;

                var mobPosition = mobEntity.Position.CurrentValue;
                if (MobDistanceShotCastle(mobPosition))
                {
                    castleIsShooting = true;
                    _shotService.CreateShotCastle(mobEntity, castleTowerOnePosition, _gameplayState.Castle.Damage);
                    _shotService.CreateShotCastle(mobEntity, castleTowerTwoPosition, _gameplayState.Castle.Damage);
                    break; //Для не мульти башни 1 выстрел
                }
            }

            if (!castleIsShooting) //За обход башня не выстрелила, таймер КД не запускаем
            {
                _gameplayState.Castle.IsShot.Value = false;
                yield break;
            }

            yield return
                new WaitForSeconds(_gameplayState.Castle.Speed /
                                   _gameplayState.GameSpeed.Value); //Поделить на тек. скорость игры
            _gameplayState.Castle.IsShot.Value = false;
        }

        public IEnumerator TowerFireShot(TowerEntity towerEntity)
        {
            yield return new WaitUntil(() => !_fsmGameplay.IsGamePause.Value); //На паузе не стреляем

            if (!towerEntity.Parameters.TryGetValue(TowerParameterType.Speed, out var towerSpeed)) yield break;
            var towerPosition = towerEntity.Position.CurrentValue;
            var towerIsShooting = false;
            //Обходим список мобов
            foreach (var keyMobEntity in _waveService.AllMobsMap)
            {
                var mobEntity = keyMobEntity.Value;
                if (!towerEntity.IsTargetForAttack(mobEntity.IsFly))
                    continue; //Проверка на совпадение типа врага и башни

                var mobPosition = mobEntity.Position.CurrentValue;
                if (MobDistanceShot(mobPosition, towerPosition, towerEntity.Parameters))
                {
                    towerIsShooting = true;
                    _shotService.CreateShot(towerEntity, mobEntity); //Создать выстрел
                    if (!towerEntity.IsMultiShot) break; //Для не мульти башни 1 выстрел
                }
            }

            if (!towerIsShooting) //За обход башня не выстрелила, таймер КД не запускаем
            {
                towerEntity.IsShot.Value = false;
                yield break;
            }

            yield return
                new WaitForSeconds(towerSpeed.Value / _gameplayState.GameSpeed.Value); //Поделить на тек. скорость игры
            towerEntity.IsShot.Value = false;
        }


        private bool MobDistanceShot(Vector2 mobPosition, Vector2Int towerPosition,
            Dictionary<TowerParameterType, TowerParameterData> parameters)
        {
            var d = Vector2.Distance(mobPosition, towerPosition) - 0.5f; //Отнимаем радиус башни
            //У башни мин. и макс. дистанция
            if (parameters.TryGetValue(TowerParameterType.MinDistance, out var distanceMin) &&
                parameters.TryGetValue(TowerParameterType.MaxDistance, out var distanceMax))
                return distanceMin.Value <= d && d <= distanceMax.Value;
            // у башни стандартная дистанция 
            if (parameters.TryGetValue(TowerParameterType.Distance, out var distance))
            {
                return d <= distance.Value;
            }

            var start = new Vector3(towerPosition.x, 0.1f, towerPosition.y);
            var end = new Vector3(towerPosition.x + 0.5f, 0.1f, towerPosition.y);
            Debug.DrawLine(start, end);

            //Башня на дороге, нет дистанции
            return Vector2.Distance(mobPosition, towerPosition) <= 0.5f;
        }

        private bool MobDistanceShotCastle(Vector2 mobPosition)
        {
            //Первичная проверка 
            if (mobPosition.x > 3) return false;
            return mobPosition.y is <= 1.5f and >= -1.5f;
            //TODO Сделать проверку на точку пути 
        }
    }
}