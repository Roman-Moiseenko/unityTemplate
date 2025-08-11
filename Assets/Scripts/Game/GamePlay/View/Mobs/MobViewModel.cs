using System.Collections;
using System.Collections.Generic;
using Game.Common;
using Game.GamePlay.Classes;
using Game.GamePlay.Services;
using Game.State.Maps.Mobs;
using Game.State.Maps.Roads;
using ObservableCollections;
using R3;
using Scripts.Utils;
using UnityEngine;

namespace Game.GamePlay.View.Mobs
{
    public class MobViewModel
    {
        private MobEntity _mobEntity;
        private int _currentIndexListPoint = 0;
        private Vector2 _targetPosition;
        private readonly WaveService _waveService;
        public int MobEntityId => _mobEntity.UniqueId;
        public bool IsFly => _mobEntity.IsFly;
        public string ConfigId => _mobEntity.ConfigId;
        public ReactiveProperty<bool> IsMoving = new(false);

        public ReactiveProperty<Vector2> Position => _mobEntity.Position;
        public ReactiveProperty<Vector2Int> Direction;
        public readonly ReactiveProperty<int> GameSpeed;
        
        public Vector2 StartPosition;
        public Vector2Int StartDirection;
        public List<RoadPoint> RoadPoints = new();
        public GameplayCamera CameraService;
        public ReactiveProperty<MobState> State;
        public ReactiveProperty<float> CurrentHealth;
        public float MaxHealth;
        public float Delta => _mobEntity.Delta;
        public ReactiveProperty<bool> FinishCurrentAnimation = new(true);
        public ReactiveProperty<bool> AnimationDelete = new(false);
        public IReadOnlyObservableDictionary<string, MobDebuff> Debuffs => _mobEntity.Debuffs;
        public int Level => _mobEntity.Level;
        public float Attack => _mobEntity.Attack;
        //public float SpeedAttack => _mobEntity.SpeedAttack;

        public MobViewModel(MobEntity mobEntity, WaveService waveService, GameplayCamera cameraService)
        {
            _mobEntity = mobEntity;
            _waveService = waveService;
            CameraService = cameraService;
            StartPosition = mobEntity.Position.CurrentValue;
            StartDirection = mobEntity.Direction.CurrentValue;
            GameSpeed = waveService.GameSpeed;
            CurrentHealth = mobEntity.Health;
            MaxHealth = mobEntity.Health.CurrentValue;

            State = mobEntity.State;
            Direction = new ReactiveProperty<Vector2Int>(mobEntity.Direction.CurrentValue); //Начальное направление
        }

        public IEnumerator WaitFinishAnimation()
        {
            Debug.Log(FinishCurrentAnimation.Value);
            while (!FinishCurrentAnimation.Value)
            {
                yield return null;
            }
            Debug.Log(FinishCurrentAnimation.Value);
        }
        
        public IEnumerator TimerDebuff(string configId, MobDebuff debuff)
        {
            //Пауза
            yield return new WaitForSeconds(debuff.Time);
            _mobEntity.RemoveDebuff(configId);
        }

        public IEnumerator RotateModel(Vector2Int direction)
        {
            
            yield return null;
        }

        public IEnumerator MovingModel(List<RoadPoint> roadPoints)
        {
            RoadPoints = roadPoints;
            _targetPosition = GetTargetPosition();
            IsMoving.Value = true; //Начать движение
            while (IsMoving.Value)
            {
                yield return MovingEntity();
            }
            
            yield return new WaitUntil(() => !IsMoving.CurrentValue); // 
        }

        private IEnumerator MovingEntity()
        {
            if (IsMoving.Value)
            {
                if (_targetPosition == Position.CurrentValue) //Дошли то след.точки
                {
                    Direction.Value = RoadPoints[_currentIndexListPoint].Direction; //Направление поворота
                    //Проверяем, поменялось ли направление
                    _currentIndexListPoint++;
                    if (_currentIndexListPoint == RoadPoints.Count)
                    {
                        IsMoving.Value = false;
                        yield break;
                    }
                    _targetPosition = GetTargetPosition();
                }
                
                var speedMob = GameSpeed.CurrentValue * AppConstants.MOB_BASE_SPEED * _mobEntity.Speed();
                Position.Value = Vector2.MoveTowards(
                    Position.CurrentValue, 
                    _targetPosition,  
                    Time.deltaTime * speedMob);
            }
            yield return null;
        }
        
        private Vector2 GetTargetPosition()
        {
            var newValue = RoadPoints[_currentIndexListPoint].Point;
            _targetPosition = new Vector2(newValue.x, newValue.y);
            return _targetPosition;
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
            State.Value = MobState.Attacking;
            
            
        }
    }
}