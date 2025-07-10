using System.Collections;
using System.Collections.Generic;
using Game.Common;
using Game.GamePlay.Services;
using Game.State.Maps.Mobs;
using ObservableCollections;
using R3;
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
        public ReactiveProperty<Vector2Int> Direction => _mobEntity.Direction;
        public readonly ReactiveProperty<int> GameSpeed;
        public ReactiveProperty<float> MobSpeed = new(1.0f); //TODO Перенести в моб энтиити
        public Vector2 StartPosition;
        public Vector2Int StartDirection;
        public List<Vector2> RoadPoints = new();
        
        public MobViewModel(MobEntity mobEntity, WaveService waveService)
        {
            _mobEntity = mobEntity;
            _waveService = waveService;
            StartPosition = mobEntity.Position.CurrentValue;
            StartDirection = mobEntity.Direction.CurrentValue;
            GameSpeed = waveService.GameSpeed;
         //   Debug.Log("Создаем View Model для " + ConfigId + " MobEntityId " + MobEntityId);
            //Position.Subscribe(p => Debug.Log("p = " + p));
            //Debug.Log("mobEntity.Origin.UniqueId = " + mobEntity.Origin.UniqueId);
          /*  IsMoving.Subscribe(newValue =>
            {
                Debug.Log(" Моб = " + ConfigId + " движется = " + newValue + " в сторону " + Position.CurrentValue);
            });*/
            //TODO Заполняем данными модель
        }

        public void SetStartPosition(Vector2 positionCurrentValue, Vector2Int directionCurrentValue)
        {
            
        }

        public IEnumerator RotateModel(Vector2Int direction)
        {
            
            yield return null;
        }

        public IEnumerator MovingModel(List<Vector2> roadPoints)
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
                if (_targetPosition == Position.CurrentValue)
                {
                    _currentIndexListPoint++;
                    if (_currentIndexListPoint == RoadPoints.Count)
                    {
                        IsMoving.Value = false;
                        yield break;
                    }
                    _targetPosition = GetTargetPosition();
                }
                
                var speedMob = GameSpeed.CurrentValue * AppConstants.MOB_BASE_SPEED * MobSpeed.CurrentValue;
                Position.Value = Vector3.MoveTowards(Position.CurrentValue, _targetPosition,  Time.deltaTime * speedMob);
            }
            yield return null;
        }
        
        private Vector3 GetTargetPosition()
        {
            var newValue = RoadPoints[_currentIndexListPoint];
            _targetPosition = new Vector3(newValue.x, newValue.y);
            return _targetPosition;
        }
    }
}