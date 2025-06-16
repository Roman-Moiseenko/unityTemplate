using Game.GamePlay.Controllers;
using UnityEngine;

namespace Game.GamePlay.Classes
{
    public class GameplayCamera
    {
        public Camera Camera;
        public Transform CameraSystem;
        
        public float MoveSpeed = 4f;
        public float Sensitivity = 0.5f;
        public float SensTouch = 0.1f;

        private float _tempSens;
        private bool _isDragging = false, _isMoving = false;
        private Vector2 _tempCenter, _targetDirection, _tempMousePos;

        private readonly RectBorder _border;//, _cameraBorder;

      //  public bool _isAutoMoving = false;

        //public Vector3 _targetAutoMoving;

        public GameplayCamera(Camera _camera, Transform cameraSystem)
        {
            Camera = _camera;
            CameraSystem = cameraSystem;
            int _centreX = 0; //Размеры игрового мира
            int _centreY = 0;
            int width = 8;
            int height = 8;

            _border = new RectBorder(_centreX, _centreY, width, height);
            
            float _newPosX = Mathf.Clamp(_centreX, _border.BottomX, _border.TopX);
            float _newPosY = Mathf.Clamp(_centreY, _border.BottomY, _border.TopY);
            CameraSystem.transform.position = new Vector3(_newPosX, CameraSystem.transform.position.y, _newPosY);
        }
        
        public void OnPointDown(Vector2 mousePosition)
        {
            _tempCenter = GetWorldPoint(mousePosition);
            _targetDirection = Vector2.zero;
            _tempMousePos = mousePosition;
            _isDragging = true;
            _isMoving = true;
          //  Debug.Log("OnPointDown");
        }

        public void OnPointMove(Vector2 mousePosition)
        {
            if (_isDragging)
            {
                //Debug.Log("_isDragging");

                Vector2 point = GetWorldPoint(mousePosition);
                float sqrDst = (_tempCenter - point).sqrMagnitude;
                if (sqrDst > SensTouch)
                {
                    //targetDirection = mousePosition.normalized;
                    if (_tempMousePos != mousePosition)
                    {
                      //  var _targetDirection = (_tempMousePos - mousePosition).normalized;
                        _targetDirection = RotateTarget((_tempMousePos - mousePosition).normalized);
                    }
                    _tempMousePos = mousePosition;
                }
            }
        }

        private Vector2 RotateTarget(Vector2 vector)
        {
            
            var angel = 135;
            var Sn = Mathf.Sin(angel * Mathf.PI / 180);
            var Cn = Mathf.Cos(angel * Mathf.PI / 180);
            return new Vector2(
                vector.x * Cn - vector.y * Sn,
                vector.x * Sn + vector.y * Cn
            );
        }

        public void OnPointUp(Vector2 mousePosition)
        {
            _isDragging = false;
            Vector2 point = GetWorldPoint(mousePosition);
            float sqrDst = (_tempCenter - point).sqrMagnitude;
            if (sqrDst <= SensTouch) _isMoving = false;
        }

        public bool Is_Moving()
        {
            return _isDragging;
        }
        
        public void UpdateMoving()
        {
            if (!_isMoving) return;

            float speed = Time.deltaTime * MoveSpeed;
            if (_isDragging)
            {
                _tempSens = Sensitivity;
            }
            else if (_tempSens > SensTouch)
            {
                _tempSens = Mathf.Lerp(_tempSens, 0f, speed / 5);
            }

            if (_tempSens <= SensTouch) _isMoving = false;
            Vector3 newPosition = CameraSystem.transform.position + new Vector3(_targetDirection.x, 0, _targetDirection.y) * _tempSens;
            newPosition.x = Mathf.Clamp(newPosition.x, _border.BottomX, _border.TopX);
            newPosition.z = Mathf.Clamp(newPosition.z, _border.BottomY, _border.TopY);
            
            CameraSystem.transform.position = Vector3.Lerp(CameraSystem.transform.position, newPosition, speed);
        }
        
        public Vector2 GetWorldPoint(Vector2 mousePosition)
        {
            Vector3 point = Camera.ScreenToWorldPoint(mousePosition);
            return new Vector2(point.x, point.z);
            /*

            Vector2 point = Vector2.zero;
            Ray ray = _camera.ScreenPointToRay(mousePosition);
            Vector3 normal = Vector3.forward;
            Vector3 position = Vector3.zero;
            Plane plane = new Plane(normal, position);
            float distance;
            plane.Raycast(ray, out distance);
            point = ray.GetPoint(distance);
            return point;*/
        }
    }
}