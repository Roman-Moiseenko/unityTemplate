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
    /**
     * Сервис проверки башен для атаки и запуск процедур атаки, бафа или создания войск
     * И нанесения урона мобам после удачной атаки
     * З.Ы. Переименовать в соответствующее название ... ??? TowerAttackService ??
     */
    public class DamageService
    {
        public ObservableList<DamageEntity> AllDamages = new();

        private readonly FsmGameplay _fsmGameplay;
        private readonly WaveService _waveService;
        private readonly WarriorService _warriorService;
        private readonly Coroutines _coroutines;
        private readonly CastleEntity _castle;
        private readonly ObservableList<TowerEntity> _allTowers;


        public DamageService(
            FsmGameplay fsmGameplay,
            GameplayStateProxy gameplayState,
            WaveService waveService,
            TowersService towersService,
            RewardProgressService rewardProgressService,
            WarriorService warriorService
        )
        {
            _fsmGameplay = fsmGameplay;
            _waveService = waveService;
            _warriorService = warriorService;
            _coroutines = GameObject.Find("[COROUTINES]").GetComponent<Coroutines>();
            _castle = gameplayState.Castle;
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
                            rewardProgressService.RewardKillMob(mobEntity.RewardCurrency, mobEntity.Position.CurrentValue);
                            waveService.AllMobsMap.Remove(e.Value.Key);
                            
                            //Ищем башни в целях которых был моб и удаляем
                            foreach (var towerViewModel in towersService.AllTowers.ToList())
                            {
                                foreach (var target in towerViewModel.Targets.ToList())
                                {
                                    //if (target.UniqueId == mobEntity.UniqueId)
                                    towerViewModel.TowerEntity.RemoveTarget(target); //Моб умер - Удаляем из башни цель
                                }
                            }
                            
                            _castle.ClearTarget(); //Удаляем из цели крепости
                        }
                    );
            });
            //Подписываемся на все башни, и на Завершение выстрела
            towersService.AllTowers.ObserveAdd().Subscribe(e =>
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
                _coroutines.StartCoroutine(towerEntity.IsPlacement
                    ? TowerForPlacement(towerEntity)
                    : TowerFireShot(towerEntity));
            }
            
            if (!_castle.IsBusy.Value) //Стрельба крепости
            {
                _castle.IsBusy.Value = true;
                _coroutines.StartCoroutine(CastleFireShot());
            }
        }

        public IEnumerator CastleFireShot()
        {
            //Обходим список мобов
            foreach (var (key, mobEntity) in _waveService.AllMobsMap)
            {
                if (_castle.SetTarget(mobEntity)) break;
            }

            if (_castle.Target.Count > 0) //Была добавлена цель(и)
                yield return new WaitForSeconds(_castle.Speed); //Задержка для следующего выстрела
            //Освобождаем крепость для следующего выстрела
            _castle.IsBusy.Value = false;
        }

        private IEnumerator TowerFireShot(TowerEntity towerEntity)
        {
            //У башни нет скорости, пропускаем обработку
            if (!towerEntity.Parameters.TryGetValue(TowerParameterType.Speed, out var towerSpeed)) yield break;
            towerEntity.IsBusy.Value = true; //Башня занята, обрабатывается выстрел
            foreach (var (index, mobEntity) in _waveService.AllMobsMap)
            {
                if (towerEntity.SetTarget(mobEntity) && !towerEntity.IsMultiShot) break;
            }

            if (towerEntity.Targets.Count > 0) //Была добавлена цель(и)
                yield return
                    new WaitForSeconds(towerSpeed.Value); //Задержка для следующего выстрела

            towerEntity.IsBusy.OnNext(false); //Освобождаем башню для следующего выстрела
        }

        private IEnumerator TowerForPlacement(TowerEntity towerEntity)
        {
            towerEntity.IsBusy.Value = true;
            yield return new WaitForSeconds(1f);
            if (_warriorService.AllWarriorsIDead(towerEntity.UniqueId))
            {
                Debug.Log("Добавляем воинов на карту");
                _warriorService.AddWarriorsTower(towerEntity);    
            }
            
            //TODO Проверяем если все Warriors IsDead => Создаем новых
            towerEntity.IsBusy.OnNext(false);
        }
        
    }
}