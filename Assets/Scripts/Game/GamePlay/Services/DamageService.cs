using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.GamePlay.Fsm;
using Game.Settings.Gameplay.Entities.Tower;
using Game.State.Maps.Mobs;
using Game.State.Maps.Shots;
using Game.State.Maps.Towers;
using Game.State.Root;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using Scripts.Utils;
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

            waveService.AllMobsMap.ObserveAdd().Subscribe(e =>
            {
                var mobEntity = e.Value.Value;
                //Проверяем, что моб мертв 
                mobEntity.IsDead.Skip(1)
                    .Where(x => x)
                    .Subscribe(_ =>
                        {
                            //выдаем награды и другое 
                            rewardProgressService.RewardKillMob(mobEntity.RewardCurrency,
                                mobEntity.Position.CurrentValue);
                            waveService.AllMobsMap.Remove(e.Value.Key);
                            //Ищем башни в целях которых был моб и удаляем
                            foreach (var towerViewModel in _towersService.AllTowers.ToList())
                            {
                                foreach (var target in towerViewModel.Targets.ToList())
                                {
                                    if (target.UniqueId == mobEntity.UniqueId)
                                    {
                                        Debug.Log("Моб умер - Удаляем из башни цель");
                                        towerViewModel.RemoveTarget(target);
                                    }
                                }
                            }
                        }
                    );
            });

            _towersService.AllTowers.ObserveAdd().Subscribe(e =>
            {
                var towerViewModel = e.Value;
                towerViewModel.Targets.ObserveRemove().Subscribe(v =>
                {
//                    Debug.Log("для башни " + towerViewModel.TowerEntityId + " поражение моба " + v.Value.UniqueId);
                    var mobEntity = v.Value;
                    var shot = towerViewModel.GetShotParameters(mobEntity);
                    if (towerViewModel.IsSingleTarget) //Одиночная цель
                    {
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
                    else
                    {
                        //Найти всех мобов в радиусе поражения и проверит на совместимость воздух/земля 
                        //и нанести каждому урон
                        
                        var position = mobEntity.Position.CurrentValue;
                        var typeDamage = shot.DamageType == DamageType.Normal ? DamageType.MassDamage : shot.DamageType;
                        var mobsUnderAttacks = new List<MobEntity>();
                        //Ищем соучастников урона
                        foreach (var entity in _waveService.AllMobsMap)
                        {
                            //  var mobEntityUnderAttack = entity.Value;
                            if (Vector2.Distance(position, entity.Value.GetPosition()) <= 0.5f)
                            {
                                mobsUnderAttacks.Add(entity.Value);
                            }
                        }

                        foreach (var mobUnderAttack in mobsUnderAttacks)
                        {
                            var damage = new DamageEntity
                            {
                                Position = mobUnderAttack.GetPosition(),
                                Damage = Mathf.FloorToInt(mobUnderAttack.SetDamage(shot.Damage)),
                                Type = typeDamage,
                            };
                            AllDamages.Add(damage);
                        }
                        
                    }
                });
            });
/*
            _shotService.Shots.ObserveRemove().Subscribe(e =>
            {
                var shot = e.Value;
              
                if (_waveService.AllMobsMap.TryGetValue(shot.MobEntityId, out var mobEntity))
                {
                    if (!shot.Single)
                    {
                        var position = mobEntity.Position.CurrentValue;
                        var typeDamage = shot.DamageType == DamageType.Normal ? DamageType.MassDamage : shot.DamageType;
                        var mobsUnderAttacks = new List<MobEntity>();
                        //Ищем соучастников урона
                        foreach (var entity in _waveService.AllMobsMap)
                        {
                            //  var mobEntityUnderAttack = entity.Value;
                            if (Vector2.Distance(position, entity.Value.GetPosition()) <= 0.5f)
                            {
                                mobsUnderAttacks.Add(entity.Value);
                            }
                        }

                        foreach (var mobUnderAttack in mobsUnderAttacks)
                        {
                            var damage = new DamageEntity
                            {
                                Position = mobUnderAttack.GetPosition(),
                                Damage = Mathf.FloorToInt(mobUnderAttack.SetDamage(shot.Damage)),
                                Type = typeDamage,
                            };
                            AllDamages.Add(damage);
                        }

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
                    //Ситуация: моб уже уничтожен, а прилетел новый выстрел от другой башни.
                    //  throw new Exception($"Моб не найден {shot.MobEntityId}");
                }
            });
*/
            //TODO Подписываемся на все башни, и на Завершение выстрела
        }

        public void Update()
        {
            if (_fsmGameplay.IsGamePause.Value) return;
            if (_waveService.AllMobsMap.Count == 0) return;
            
            foreach (var towerEntity in _gameplayState.Towers)
            {
                if (towerEntity.IsBusy.Value) continue; 
                towerEntity.IsBusy.Value = true;
                _coroutines.StartCoroutine(TowerFireShot(towerEntity));
            }

            if (!_gameplayState.Castle.IsShot.Value) //Стрельба крепости
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
            //У башни нет скорости, пропускаем обработку
            if (!towerEntity.Parameters.TryGetValue(TowerParameterType.Speed, out var towerSpeed)) yield break;
            
            towerEntity.IsBusy.Value = true; //Башня занята, обрабатывается выстрел 
            yield return TowerOneShot(towerEntity);
            /*
            if (!towerEntity.IsMultiShot) //Обрабатываем выстрел
            {
                yield return TowerOneShot(towerEntity);
            }
            else
            {
                yield return TowerMultiShot(towerEntity);
            }
            */
            yield return null; //Прерывание на кадр
            
            if (towerEntity.Targets.Count > 0) //Была добавлена цель(и)
            {
                yield return new WaitForSeconds(towerSpeed.Value / _gameplayState.GameSpeed.Value); //Задержка для следующего выстрела
            }
            
            towerEntity.IsBusy.OnNext(false); //Освобождаем башню для следующего выстрела

        }

        private IEnumerator TowerOneShot(TowerEntity towerEntity)
        {
            //Debug.Log("Обрабатываем одиночный выстрел");
            //var towerPosition = towerEntity.Position.CurrentValue;
            //if (!towerEntity.Parameters.TryGetValue(TowerParameterType.Speed, out var towerSpeed)) yield break;

            //MobEntity cacheMobEntity = null;
            foreach (var (index, mobEntity) in _waveService.AllMobsMap)
            {
                //if (!towerEntity.IsTargetForAttack(mobEntity.IsFly)) continue; //Проверка на совпадение типа врага и башни

                //var mobPosition = mobEntity.Position.CurrentValue;
                if (towerEntity.SetTarget(mobEntity) && !towerEntity.IsMultiShot)
                {
                    //Debug.Log("Задержка для следующего выстрела с=" + towerSpeed.Value / _gameplayState.GameSpeed.Value);
                    //yield return new WaitForSeconds(towerSpeed.Value / _gameplayState.GameSpeed.Value);
                    yield break;
                }

                //  if (!towerEntity.IsShot.Value && towerEntity.MobDistanceShot(mobPosition))
                //   {
                //towerEntity.IsShot.Value = true;
                //Запускаем поворот Передать вектор моба
                //towerEntity.PrepareShot.OnNext(mobPosition); //
//                      Debug.Log("Выстрел " + towerEntity.UniqueId);
                //towerEntity.IsShot.Value = true;

                //Задержка перед выстрелом

                //towerEntity.SetTarget(mobEntity);

                //cacheMobEntity = mobEntity;

                //towerEntity.Targets.Add(mobEntity);
                //    break;
                //   }
            }
//            Debug.Log("Обработка закончилась");
/*
            if (towerEntity.IsShot.Value && cacheMobEntity != null)
            {
                yield return new WaitForSeconds(towerSpeed.Value / _gameplayState.GameSpeed.Value);
                //Выстрел
                _shotService.CreateShot(towerEntity, cacheMobEntity); //Создать выстрел
            }
            */
        }

        private IEnumerator TowerMultiShot(TowerEntity towerEntity)
        {
           // if (!towerEntity.Parameters.TryGetValue(TowerParameterType.Speed, out var towerSpeed)) yield break;
            //var i = 0;
            // List<MobEntity> modsUnderAttack = new(); 
            foreach (var (index, mobEntity) in _waveService.AllMobsMap)
            {
                // if (!towerEntity.IsTargetForAttack(mobEntity.IsFly)) continue; //Проверка на совпадение типа врага и башни

                // var mobPosition = mobEntity.Position.CurrentValue;
                // if (towerEntity.MobDistanceShot(mobPosition))
                // {
                //    modsUnderAttack.Add(mobEntity); //Создаем список мобов которые попали под массовую аттаку

                towerEntity.SetTarget(mobEntity);
                //towerEntity.IsShot.Value = true;
                //towerEntity.Targets.Add(mobEntity);
                //_shotService.CreateShot(towerEntity, mobEntity); //Создать выстрел
                // }
            }
/*
            if (towerEntity.Targets.Count > 0) //Список не пуст 
            {
                yield return new WaitForSeconds(towerSpeed.Value / _gameplayState.GameSpeed.Value);
            }
*/
            /*
            if (towerEntity.IsShot.Value)
            {
                yield return new WaitForSeconds(towerSpeed.Value / _gameplayState.GameSpeed.Value);
            }
            */
            yield break;
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