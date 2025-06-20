using R3;
using UnityEngine;

namespace Game.GamePlay.View.Frames
{
    public class FrameBinder : MonoBehaviour
    {
        private FrameViewModel _frameViewModel;
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
        
        public void Bind(FrameViewModel frameViewModel)
        {
            _frameViewModel = frameViewModel;

            frameViewModel.Enable.Subscribe(newValue =>
            {
                if (newValue)
                {
                    frame.GetComponent<MeshRenderer>().material = allowed; //Debug.Log("Можно строить");
                }
                else
                {
                    frame.GetComponent<MeshRenderer>().material = forbidden; //Debug.Log("Нельзя строить");
                }
            });

            frameViewModel.IsSelected.Subscribe(newValue =>
            {
                var color = frame.GetComponent<MeshRenderer>().material.color;
                Debug.Log("color.a = " + color.a);
                if (newValue)
                {
                    //Debug.Log();
                    color.a = 125f;
                }
                else
                {
                    color.a = 111f;
//                    frame.GetComponent<MeshRenderer>().material = forbidden; //Debug.Log("Нельзя строить");
                }
                frame.GetComponent<MeshRenderer>().material.color = color;
            });
            
            _frameViewModel.Position.Subscribe(newPosition =>
            {
                _targetPosition = new Vector3(newPosition.x, 0, newPosition.y);
                _isMoving = true;
            });
            
            
            transform.position = new Vector3(
                _frameViewModel.Position.CurrentValue.x,
                0,
                _frameViewModel.Position.CurrentValue.y
            );
            
          //  transform.position = new Vector3(frameViewModel.Position.CurrentValue.x, 0, frameViewModel.Position.CurrentValue.y);
            //TODO Смена материала
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