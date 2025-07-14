using System;
using System.Collections;
using System.Collections.Generic;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.States;
using Game.GamePlay.View.Mobs;
using Game.GamePlay.View.Towers;
using Game.Settings.Gameplay.Entities.Tower;
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
        // private 

        public DamageService(
            FsmGameplay fsmGameplay,
            GameplayStateProxy gameplayState,
            TowersSettings towersSettings,
            WaveService waveService,
            TowersService towersService,
            ShotService shotService
        )
        {
            _fsmGameplay = fsmGameplay;
            _gameplayState = gameplayState;
            _towersSettings = towersSettings;
            _waveService = waveService;
            _towersService = towersService;
            _shotService = shotService;
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
                            gameplayState.Progress.Value += 5;
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
                        mobEntity.SetDamage(shot.Damage);    
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

            foreach (var towerEntity in _gameplayState.Towers)
            {
                //Debug.Log("towerEntity.IsShot.Value = " + towerEntity.IsShot.Value);
                if (!towerEntity.IsShot.Value) //Если башня не стреляет, запускаем корутин
                {
                    //Debug.Log(Time.deltaTime);
                    towerEntity.IsShot.Value = true;
                    _coroutines.StartCoroutine(TowerFireShot(towerEntity));
                }
            }

            //  });
        }


        public IEnumerator TowerFireShot(TowerEntity towerEntity)
        {
            yield return new WaitUntil(() => !_fsmGameplay.IsGamePause.Value); //На паузе не стреляем
            
            if (!towerEntity.Parameters.TryGetValue(TowerParameterType.Speed, out var towerSpeed)) yield break;
            
            //                Debug.Log("towerEntity.IsMultiShot = " + towerEntity.IsMultiShot);
            //Обходим список мобов
            foreach (var keyMobEntity in _waveService.AllMobsMap)
            {
                var mobEntity = keyMobEntity.Value;
                if (!towerEntity.IsTargetForAttack(mobEntity.IsFly)) continue; //Проверка на совпадение типа врага и башни

                var mobPosition = mobEntity.Position.CurrentValue;
                var towerPosition = towerEntity.Position.CurrentValue;
                if (MobDistanceShot(mobPosition, towerPosition, towerEntity.Origin.Parameters))
                {
                    _shotService.CreateShot(towerEntity, mobEntity); //Создать выстрел
                }
               
                if (!towerEntity.IsMultiShot) break;
            }
            
            yield return new WaitForSeconds(towerSpeed.Value.Value / _gameplayState.GameSpeed.Value);//TODO Поделить на тек. скорость игры
            towerEntity.IsShot.Value = false;
        }


        private bool MobDistanceShot(Vector2 mobPosition, Vector2Int towerPosition,
            Dictionary<TowerParameterType, TowerParameterData> parameters)
        {
            var d = Vector2.Distance(mobPosition, towerPosition) - 0.5f; //Отнимаем радиус башни

            if (parameters.TryGetValue(TowerParameterType.Distance, out var distance))
                return d <= distance.Value;
            
            if (parameters.TryGetValue(TowerParameterType.MinDistance, out var distanceMin) &&
                parameters.TryGetValue(TowerParameterType.MaxDistance, out var distanceMax))
                return distanceMin.Value <= d && d <= distanceMax.Value;
            
            return false;
        }
    }
}