using Newtonsoft.Json;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Roads
{
    public class RoadBinder : MonoBehaviour
    {
        private Vector3 _targetPosition;
        private RoadViewModel _viewModel;
        private bool _isMoving = false;
        
        private const int speed = 20;
        private const float smoothTime = 0.2f;
        private Vector3 _velocity;
        
        public void Bind(RoadViewModel viewModel)
        {
            _viewModel = viewModel;


            viewModel.Rotate.Subscribe(newValue =>
            {
                transform.localEulerAngles = new Vector3(0, 90f * newValue,0);
            });
            
            viewModel.Position.Subscribe(newPosition =>
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
                transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref _velocity, smoothTime, speed );
                if (_velocity.magnitude < 0.0005)
                {
                    _isMoving = false;
                    transform.position = _targetPosition;
                }
            
            }
        }
        
    }
    
    
}