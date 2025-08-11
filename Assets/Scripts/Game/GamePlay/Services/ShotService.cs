using System;
using System.Collections;
using System.Collections.Generic;
using DI;
using Game.GamePlay.Fsm;
using Game.GamePlay.View.Shots;
using Game.GamePlay.View.Towers;
using Game.Settings.Gameplay.Entities.Tower;
using Game.State.Maps.Mobs;
using Game.State.Maps.Shots;
using Game.State.Maps.Towers;
using Game.State.Root;
using ObservableCollections;
using R3;
using Scripts.Utils;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Game.GamePlay.Services
{
    public class ShotService
    {
        private readonly GameplayStateProxy _gameplayState;
        private readonly TowersSettings _towersSettings;
        private readonly FsmGameplay _fsmGameplay;
        private readonly Coroutines _coroutines;
        private int _uniqueShotId = 0;
        
        public ObservableList<ShotEntity> Shots = new();
        public ObservableList<ShotViewModel> AllShots = new();
        private Dictionary<int, ShotViewModel> _shotsMap = new();
        private readonly Dictionary<string, ShotSettings> _shotSettingsMap = new(); //Кешируем настройки выстрелов
        
        public readonly ReactiveProperty<int> GameSpeed = new(1);
        //TODO Нужны ли данные в конструктор? Возможно настройки базовые?
        public ShotService(GameplayStateProxy gameplayState, TowersSettings towersSettings, FsmGameplay fsmGameplay)
        {
            _gameplayState = gameplayState;
            _towersSettings = towersSettings;
            _fsmGameplay = fsmGameplay;
            GameSpeed = gameplayState.GameSpeed;
            _coroutines = GameObject.Find("[COROUTINES]").GetComponent<Coroutines>();
            foreach (var towerSettings in towersSettings.AllTowers)
            {
                _shotSettingsMap.Add(towerSettings.ConfigId, towerSettings.Shot);
            }

            AllShots.ObserveAdd().Subscribe(newValue =>
            {
                _coroutines.StartCoroutine(MovingShot(newValue.Value)); 
            });
            AllShots.ObserveRemove().Subscribe(value =>
            {
                _coroutines.StopCoroutine(MovingShot(value.Value));
            });
        }


        public void CreateShot(TowerEntity towerEntity, MobEntity mobEntity)
        {
            var startPosition =
                new Vector3(towerEntity.Position.CurrentValue.x, 1f, towerEntity.Position.CurrentValue.y);
            
            var damage = 0f;
            //Расчет урона от башни
            if (towerEntity.Parameters.TryGetValue(TowerParameterType.Damage, out var parameter))
            {
                damage = parameter.Value;
                //Single = true
            }

            MobDebuff debuff = null;
            if (towerEntity.Parameters.TryGetValue(TowerParameterType.DamageArea, out parameter))
            {
                damage = parameter.Value;
                //TODO Возможно перенести Single = false
            }

            //Добавляем дебафф к выстрелу
            if (towerEntity.Parameters.TryGetValue(TowerParameterType.SlowingDown, out var slowParameter))
            {
                var speedTower = 1f;
                if (towerEntity.Parameters.TryGetValue(TowerParameterType.Speed, out var speedParameter))
                    speedTower = speedParameter.Value; //Скорость выстрела == время действия дебафа
                
                debuff = new MobDebuff
                {
                    Value = slowParameter.Value,
                    Type = MobDebuffType.Speed,
                    Time = speedTower,
                };
            }
            //Critical damage
            var damageType = DamageType.Normal;
            if (towerEntity.Parameters.TryGetValue(TowerParameterType.Critical, out var criticalParameter))
            {
                var shans = Mathf.FloorToInt(100 / criticalParameter.Value);
                if (Mathf.FloorToInt(Mathf.Abs(Random.insideUnitSphere.x) * 999) % shans == 0)
                {
                    damageType = DamageType.Critical;
                    damage *= 2.0f;
                }
            }
            
            var shotEntityData = new ShotEntityData
            {
                TowerEntityId = towerEntity.UniqueId,
                MobEntityId = mobEntity.UniqueId,
                ConfigId = towerEntity.ConfigId,
                StartPosition = startPosition,
                Position = startPosition,
                UniqueId = GetUniqueId(),
                Speed = _shotSettingsMap[towerEntity.ConfigId].Speed, 
                Single = _shotSettingsMap[towerEntity.ConfigId].Single,
                Damage = damage, 
                NotPrefab = _shotSettingsMap[towerEntity.ConfigId].NotPrefab,
                Debuff = debuff,
                DamageType = damageType,
            };

            CreateShotEntity(shotEntityData, mobEntity.PositionTarget);

        }

        public void CreateShotCastle(MobEntity mobEntity, Vector2Int position, float damage)
        { 
            var startPosition = new Vector3(position.x, 0.5f, position.y);
            //Critical damage
            
            var shotEntityData = new ShotEntityData
            {
                TowerEntityId = -1,
                MobEntityId = mobEntity.UniqueId,
                ConfigId = "Castle01",
                StartPosition = startPosition,
                Position = startPosition,
                UniqueId = GetUniqueId(),
                Speed = 1f, 
                Single = true,
                Damage = damage, 
                NotPrefab = false,
                Debuff = null,
                DamageType = DamageType.Normal,
            };

            CreateShotEntity(shotEntityData, mobEntity.PositionTarget);
        }

        private void CreateShotEntity(ShotEntityData shotEntityData, ReactiveProperty<Vector3> position)
        {
            var shotEntity = new ShotEntity(shotEntityData, position);
            Shots.Add(shotEntity); //Удалить
            var shotViewModel = new ShotViewModel(shotEntity, this);
            _shotsMap.Add(shotEntity.UniqueId, shotViewModel);
            AllShots.Add(shotViewModel);
        }

        private void RemoteShot(ShotViewModel shotViewModel)
        {
            if (_shotsMap.TryGetValue(shotViewModel.ShotEntityId, out var viewModel))
            {
                Shots.Remove(viewModel._shotEntity); //удаляем сущность
                AllShots.Remove(viewModel); //удаляем модель
                _shotsMap.Remove(shotViewModel.ShotEntityId);
            }
        }

        private int GetUniqueId()
        {
            return _uniqueShotId++;
        }

        public IEnumerator MovingShot(ShotViewModel shotViewModel)
        {
            //Debug.Log("MovingShot Start");
            yield return _fsmGameplay.WaitPause();
            //yield return new WaitUntil(() => !_fsmGameplay.IsGamePause.Value);
            //Debug.Log(shotViewModel.StartPosition + " => " + shotViewModel._shotEntity.FinishPosition.CurrentValue);
            yield return shotViewModel.MovingModel();
            RemoteShot(shotViewModel);
        }
        
        
    }
}