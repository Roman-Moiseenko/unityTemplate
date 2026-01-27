using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Common;
using Game.GamePlay.Classes;
using Game.GamePlay.Fsm;
using Game.GamePlay.Services;
using Game.State.Maps.Mobs;
using Game.State.Maps.Roads;
using Game.State.Maps.Warriors;
using Game.State.Root;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using Scripts.Utils;
using UnityEngine;

namespace Game.GamePlay.View.Mobs
{
    public class MobViewModel
    {
        private MobEntity _mobEntity;


        public int UniqueId => _mobEntity.UniqueId;
        public bool IsFly => _mobEntity.IsFly;
        public string ConfigId => _mobEntity.ConfigId;
        public ReactiveProperty<bool> IsMoving = new(false);
        public ReactiveProperty<bool> IsAttack = new(false);
        public ReactiveProperty<Vector2> Position => _mobEntity.Position;

        public Vector2 StartPosition;
        public Vector2Int StartDirection;
        public List<RoadPoint> RoadPoints = new();
        public GameplayCamera CameraService;
        //public ReactiveProperty<MobState> State; //TODO Возможно удалить или модифицировать до FSM
        public ReactiveProperty<float> CurrentHealth;
        public float MaxHealth;

        public ReactiveProperty<bool> FinishCurrentAnimation = new(true);
        public ReactiveProperty<bool> AnimationDelete = new(false);
        public IReadOnlyObservableDictionary<string, MobDebuff> Debuffs => _mobEntity.Debuffs;
        public int Level => _mobEntity.Level;
        public float Damage => _mobEntity.Damage;

        public ReactiveProperty<Vector3> PositionTarget => _mobEntity.PositionTarget;
        public ReadOnlyReactiveProperty<bool> IsDead => _mobEntity.IsDead;
        public MobDefence Defence => _mobEntity.Defence;

        private readonly GameplayStateProxy _gameplayState;

        public ReactiveProperty<bool> StartGo = new(false);
        
        public MobViewModel(
            MobEntity mobEntity,
            GameplayCamera cameraService,
            GameplayStateProxy gameplayState,
            WaveService waveService
        )
        {
            _gameplayState = gameplayState;
            _mobEntity = mobEntity;
            CameraService = cameraService;
            CurrentHealth = mobEntity.Health;
            MaxHealth = mobEntity.Health.CurrentValue;
            //State = mobEntity.State;
            
            //Моб вышел на дорогу, просчитываем путь и начальные координаты, от расположения ворот
            mobEntity.IsWentOut.Where(x => x).Subscribe(_ =>
            {
                var position = waveService.GateWaveViewModel.Position.Value;
                var direction = -1 * waveService.GateWaveViewModel.Direction.Value;
                
                mobEntity.SetStartPosition(position, direction);
                StartPosition = mobEntity.Position.CurrentValue;
                StartDirection = mobEntity.Direction.CurrentValue;
                
                RoadPoints = waveService.GenerateRoadPoints(mobEntity);
                //Debug.Log(JsonConvert.SerializeObject(RoadPoints, Formatting.Indented));
                StartGo.Value = true;
                IsMoving.Value = true;
            });
        }

        public IEnumerator WaitFinishAnimation()
        {
            while (!FinishCurrentAnimation.Value)
            {
                yield return null;
            }
        }
        
        public IEnumerator TimerDebuff(string configId, MobDebuff debuff)
        {
            //Пауза
            yield return new WaitForSeconds(debuff.Time);
            _mobEntity.RemoveDebuff(configId);
        }
        

        public float GetSpeedMob()
        {
            return _mobEntity.Speed();
        }

        public Vector3 GetTargetPosition(int index)
        {
            var newValue = RoadPoints[index].Point; //_currentIndexListPoint
            return new Vector3(newValue.x, IsFly ? 0.9f : 0.0f, newValue.y);
        }

        public void StartAnimationDelete()
        {
            FinishCurrentAnimation.Value = false;
            AnimationDelete.Value = true;
        }

        public void RemoveDebuff(string configId)
        {
            _mobEntity.RemoveDebuff(configId);
        }

        public IEnumerator AttackCastle()
        {
            IsMoving.Value = false;
            IsAttack.Value = true;
            yield return AttackEntity(_gameplayState.Castle);
        }

        public IEnumerator AttackWarrior(int warriorUniqueId)
        {
            WarriorEntity warrior = null;
            foreach (var warriorEntity in _gameplayState.Warriors)
                if (warriorEntity.UniqueId == warriorUniqueId) warrior = warriorEntity;
            if (warrior == null) yield break;
            
            IsMoving.Value = false;
            IsAttack.Value = true;
            
            yield return AttackEntity(warrior);
        }

        public IEnumerator AttackEntity(IEntityHasHealth entity)
        {
            //State.Value = MobState.Attacking;
            while (IsAttack.Value)
            {
                yield return new WaitForSeconds(_mobEntity.SpeedAttack / AppConstants.MOB_SPEED_ATTACK);
                if (_mobEntity.IsDead.CurrentValue) yield break;
                entity.DamageReceived(Damage);
                if (!entity.IsDeadEntity()) continue;

                //State.Value = MobState.Moving;
                IsMoving.Value = true;
                IsAttack.Value = false;
            }
        }
    }
}