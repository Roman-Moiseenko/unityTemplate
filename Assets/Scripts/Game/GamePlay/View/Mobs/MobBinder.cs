using System;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Mobs
{
    public class MobBinder : MonoBehaviour
    {
		public MobViewModel _viewModel;
        private Vector3 _targetPosition;
        //TODO Скорость взять у моба + скорость игры из WaveService подписка
        //private readonly float _baseSpeed = 0.5f;
        private float _mobY;
        
        private int _currentIndexListPoint = 0;
    //    private bool _isMoving;
        public void Bind(MobViewModel viewModel)
        {
            _viewModel = viewModel;

            _mobY = _viewModel.IsFly ? 0.9f : 0.1f;
            transform.position = new Vector3(viewModel.StartPosition.x, _mobY, viewModel.StartPosition.y);

//            Debug.Log("viewModel.Position.CurrentValue = " + viewModel.Position.CurrentValue);
            //TODO поворачиваем модель
            _viewModel.IsMoving.Subscribe(newValue =>
            {
            });
            
            viewModel.Position.Subscribe(newValue =>
            {
                transform.position = new Vector3(newValue.x, _mobY, newValue.y);
                
              //  Debug.Log("newValue = " + newValue + " время = " + Time.time);
                
            //    _isMoving = true;
              //  _viewModel.IsMoving.Value = true;
            });
        }


        public void Update()
        {
            /*if (_viewModel.IsMoving.CurrentValue)
            {
                _targetPosition = GetTargetPosition();
                var speedMob = _viewModel.GameSpeed.CurrentValue * _baseSpeed * _viewModel.MobSpeed.CurrentValue;
                transform.position = Vector3.MoveTowards(transform.position, _targetPosition,  Time.deltaTime * speedMob);
                
                if (transform.position == _targetPosition)
                {
                 //   _isMoving = false;
                    //_viewModel.IsMoving.Value = false;
                    _currentIndexListPoint++;
                }
            }*/
        }

        private Vector3 GetTargetPosition()
        {
            var newValue = _viewModel.RoadPoints[_currentIndexListPoint];
            _targetPosition = new Vector3(newValue.x, _mobY, newValue.y);
            return _targetPosition;
        }
    }
}