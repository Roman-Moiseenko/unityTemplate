using R3;
using UnityEngine;

namespace Game.GamePlay.View.Frames
{
    public class FrameBlockBinder : MonoBehaviour
    {
        private FrameBlockViewModel _viewModel;
        
        [SerializeField] private Material allowed;
        [SerializeField] private Material allowedSelected;
        [SerializeField] private Material forbidden;
        [SerializeField] private Material forbiddenSelected;
        [SerializeField] private GameObject frame;
        
        private Vector3 _targetPosition;
        private bool _isMoving = false;
        private const int speed = 20;
        private const float smoothTime = 0.2f;
        private Vector3 _velocity;
        
        
        public void Bind(FrameBlockViewModel viewModel)
        {
            _viewModel = viewModel;
            Observable.Merge(viewModel.Enable, viewModel.IsSelected).Subscribe(v =>
            {
                SetMaterial();
            });
            viewModel.Position.Subscribe(newPosition =>
            {
                _targetPosition = new Vector3(newPosition.x, 0, newPosition.y);
                _isMoving = true;
            });


            viewModel.Rotate.Subscribe(newValue =>
            {
                transform.localEulerAngles = new Vector3(0, 90f * newValue,0);
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
        
        private void SetMaterial()
        {
            var _e = _viewModel.Enable.CurrentValue;
            var _s = _viewModel.IsSelected.CurrentValue;
            if (_e && _s)
            {
                frame.GetComponent<MeshRenderer>().material = allowedSelected; 
            }
            if (_e && !_s)
            {
                frame.GetComponent<MeshRenderer>().material = allowed; 
            }
            if (!_e && _s)
            {
                frame.GetComponent<MeshRenderer>().material = forbiddenSelected; 
            }
            if (!_e && !_s)
            {
                frame.GetComponent<MeshRenderer>().material = forbidden; 
            }
        }
    }
}