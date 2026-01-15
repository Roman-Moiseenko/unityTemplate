using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.GamePlay.Fsm;
using Game.Settings.Gameplay.Entities.Tower;
using Game.State.Maps.Castle;
using Game.State.Maps.Mobs;
using Game.State.Maps.Shots;
using Game.State.Maps.Towers;
using Game.State.Root;
using ObservableCollections;
using R3;
using Scripts.Utils;
using UnityEngine;


namespace Game.GamePlay.Services
{
    public class DamageService
    {
        private readonly FsmGameplay _fsmGameplay;
       // private readonly GameplayStateProxy _gameplayState;
        private readonly TowersSettings _towersSettings;
        private readonly WaveService _waveService;
        private readonly TowersService _towersService;
        private readonly Coroutines _coroutines;
     //   private readonly RewardProgressService _rewardProgressService;
        public ObservableList<DamageEntity> AllDamages = new();
        private readonly CastleEntity _castle;
        private readonly ObservableList<TowerEntity> _allTowers;

        private readonly ReactiveProperty<int> _gameSpeed;

        public DamageService(
            FsmGameplay fsmGameplay,
            GameplayStateProxy gameplayState,
            TowersSettings towersSettings,
            WaveService waveService,
            TowersService towersService,
            RewardProgressService rewardProgressService
        )
        {
            _fsmGameplay = fsmGameplay;
            _towersSettings = towersSettings;
            _waveService = waveService;
            _towersService = towersService;
            //_rewardProgressService = rewardProgressService;
            _coroutines = GameObject.Find("[COROUTINES]").GetComponent<Coroutines>();
            _castle = gameplayState.Castle;
            _gameSpeed = gameplayState.GameSpeed;
            _allTowers = gameplayState.Towers;
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
                                        towerViewModel.TowerEntity
                                            .RemoveTarget(target); //Моб умер - Удаляем из башни цель
                                }
                            }
                            //Удаляем из цели крепости
                            if (_castle.Target.Count > 0)
                            {
                                foreach (var entity in _castle.Target.ToList())
                                {
                                    _castle.RemoveTarget(entity);
                                }
                            }
                            
                        }
                    );
            });
            //Подписываемся на все башни, и на Завершение выстрела
            _towersService.AllTowers.ObserveAdd().Subscribe(e =>
            {
                var towerViewModel = e.Value;
                towerViewModel.Targets.ObserveRemove().Subscribe(v =>
                {
                    var mobEntity = v.Value;
                    var shot = towerViewModel.TowerEntity.GetShotParameters(mobEntity);
                    if (towerViewModel.TowerEntity.IsSingleTarget) //Одиночная цель
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
                            if (Vector2.Distance(position, entity.Value.GetPosition()) <= 0.5f)
                                mobsUnderAttacks.Add(entity.Value);
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

            _castle.Target.ObserveRemove().Subscribe(e =>
            {
                var mobEntity = e.Value;
                var damage = new DamageEntity
                {
                    Position = mobEntity.Position.CurrentValue,
                    Damage = Mathf.FloorToInt(mobEntity.SetDamage(_castle.Damage)),
                    Type = DamageType.Normal,
                };
                AllDamages.Add(damage); 
            });
        }

        public void Update()
        {
            if (_fsmGameplay.IsGamePause.Value) return;
            if (_waveService.AllMobsMap.Count == 0) return;

            foreach (var towerEntity in _allTowers)
            {
                if (towerEntity.IsBusy.Value) continue;
                towerEntity.IsBusy.Value = true;
                _coroutines.StartCoroutine(TowerFireShot(towerEntity));
            }

            if (!_castle.IsBusy.Value) //Стрельба крепости
            {
                _castle.IsBusy.Value = true;
                _coroutines.StartCoroutine(CastleFireShot());
            }
        }

        public IEnumerator CastleFireShot()
        {
            yield return new WaitUntil(() => !_fsmGameplay.IsGamePause.Value); //На паузе не стреляем

            //Обходим список мобов
            foreach (var (key, mobEntity) in _waveService.AllMobsMap)
            {
                if (_castle.SetTarget(mobEntity)) break;
            }
            if (_castle.Target.Count > 0) //Была добавлена цель(и)
                yield return new WaitForSeconds(_castle.Speed / _gameSpeed.Value); //Задержка для следующего выстрела
            //Освобождаем крепость для следующего выстрела
            _castle.IsBusy.Value = false;
        }

        public IEnumerator TowerFireShot(TowerEntity towerEntity)
        {
            yield return new WaitUntil(() => !_fsmGameplay.IsGamePause.Value); //На паузе не стреляем
            //У башни нет скорости, пропускаем обработку
            if (!towerEntity.Parameters.TryGetValue(TowerParameterType.Speed, out var towerSpeed)) yield break;
            towerEntity.IsBusy.Value = true; //Башня занята, обрабатывается выстрел
            foreach (var (index, mobEntity) in _waveService.AllMobsMap)
            {
                if (towerEntity.SetTarget(mobEntity) && !towerEntity.IsMultiShot) break;
            }

            if (towerEntity.Targets.Count > 0) //Была добавлена цель(и)
                yield return
                    new WaitForSeconds(towerSpeed.Value / _gameSpeed.Value); //Задержка для следующего выстрела

            towerEntity.IsBusy.OnNext(false); //Освобождаем башню для следующего выстрела
        }

    }
}