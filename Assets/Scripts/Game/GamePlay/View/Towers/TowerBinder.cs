
using System;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Towers
{
    public class TowerBinder : MonoBehaviour
    {
        private TowerViewModel _viewModel;
        private Vector3 _targetPosition;
        private bool _isMoving = false;
        private const int speed = 6;

        public void Bind(TowerViewModel viewModel)
        {
            _viewModel = viewModel;
            _viewModel.Position.Subscribe(newPosition =>
            {
                _targetPosition = new Vector3(newPosition.x, 0, newPosition.y);
                _isMoving = true;
            });
            
            transform.position = new Vector3(
                viewModel.Position.CurrentValue.x,
                0,
                viewModel.Position.CurrentValue.y
            );
        }

        private void Update()
        {
            if (_isMoving)
            {
                transform.position = Vector3.MoveTowards(transform.position, _targetPosition, speed * Time.deltaTime);
                if (transform.position == _targetPosition) _isMoving = false;
            }
        }
    }
    
    
}