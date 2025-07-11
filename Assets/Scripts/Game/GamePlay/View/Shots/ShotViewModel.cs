using System.Collections;
using Game.GamePlay.Services;
using Game.State.Maps.Shots;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Shots
{
    public class ShotViewModel
    {
        public readonly ShotEntity _shotEntity;
        public int ShotEntityId => _shotEntity.UniqueId;
        public string ConfigId => _shotEntity.ConfigId;
        private readonly ShotService _shotService;
        public readonly Vector3 StartPosition;
        public ReactiveProperty<Vector3> Position;
        private readonly ReactiveProperty<bool> _shotMoving;

        public ShotViewModel(ShotEntity shotEntity, ShotService shotService)
        {
            _shotEntity = shotEntity;
            _shotService = shotService;
            StartPosition = shotEntity.StartPosition;
            Position = new ReactiveProperty<Vector3>(StartPosition);
            Position.Subscribe(p => shotEntity.Position.Value = p);
            _shotMoving = new ReactiveProperty<bool>(true);
            
        }



        public IEnumerator MovingModel()
        {
            while (_shotMoving.Value)
            {
                var speedEntity = _shotService.GameSpeed.CurrentValue * _shotEntity.Speed * 10f;
               // Debug.Log("SPEED = " + speedEntity);
                //TODO Расчет координат полета от типа снаряда
                Position.Value = Vector3.MoveTowards(Position.CurrentValue, _shotEntity.FinishPosition.CurrentValue,  Time.deltaTime * speedEntity);
               // Debug.Log("Позиция Выстрела " + ShotUniqueId + " " + Position.CurrentValue);
                if (Position.CurrentValue == _shotEntity.FinishPosition.CurrentValue)
                {
                    //Выстрел достиг цели (моба) движение прекращаем, останавливается /заканчивается
                    _shotMoving.Value = false; 
                }
                yield return null;
            }
            yield return new WaitUntil(() => !_shotMoving.CurrentValue);
           // Debug.Log("Выстрел достиг цели " + ShotUniqueId);
        }
    }
}