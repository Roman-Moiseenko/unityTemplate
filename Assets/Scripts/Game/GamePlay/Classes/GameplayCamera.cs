using DI;
using Game.Common;
using Game.GamePlay.Controllers;
using Newtonsoft.Json;
using R3;
using UnityEngine;

namespace Game.GamePlay.Classes
{
    public class GameplayCamera
    {
        public Camera Camera;
        public Transform CameraSystem;

        public float MoveSpeed = 4f;
        public float Sensitivity = 1.7f;
        public float SensTouch = 0.4f;

        private const int speed = 10;
        private const float smoothTime = 0.2f;
        private Vector3 _velocity;


        private float _tempSens;
        private bool _isDragging = false, _isMoving = false;
        private Vector2 _tempCenter, _targetDirection, _tempMousePos;

        private bool _autoMoving = false;
        private Vector3 _targetAutoMoving;

        private readonly RectBorder _border; //, _cameraBorder;
        private readonly Subject<Unit> _subjectCameraMoving;


        public GameplayCamera(DIContainer container)
        {
            _subjectCameraMoving = container.Resolve<Subject<Unit>>(AppConstants.CAMERA_MOVING);
            CameraSystem = GameObject.Find("CameraSystem").GetComponent<Transform>();
            Camera = GameObject.Find("Main Camera").GetComponent<Camera>();

            int _centreX = 2; //Размеры игрового мира
            int _centreY = 2;
            int width = AppConstants.WIDTH_MAP;
            int height = AppConstants.HIGHT_MAP;

            _border = new RectBorder(_centreX, _centreY, width, height);
//            Debug.Log(JsonConvert.SerializeObject(_border, Formatting.Indented));

            float _newPosX = Mathf.Clamp(_centreX, _border.BottomX, _border.TopX);
            float _newPosY = Mathf.Clamp(_centreY, _border.BottomY, _border.TopY);
            CameraSystem.transform.position = new Vector3(_newPosX, CameraSystem.transform.position.y, _newPosY);
            _subjectCameraMoving.OnNext(Unit.Default);
        }

        public void OnPointDown(Vector2 mousePosition)
        {
            _tempCenter = GetWorldPoint(mousePosition);
            _targetDirection = Vector2.zero;
            _tempMousePos = mousePosition;
            _isDragging = true;
            _isMoving = true;
        }

        public void OnPointMove(Vector2 mousePosition)
        {
            if (_isDragging)
            {
                Vector2 point = GetWorldPoint(mousePosition);
                float sqrDst = (_tempCenter - point).sqrMagnitude;
                if (sqrDst > SensTouch)
                {
                    if (_tempMousePos != mousePosition)
                    {
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
            Vector3 newPosition = CameraSystem.transform.position +
                                  new Vector3(_targetDirection.x, 0, _targetDirection.y) * _tempSens;
            newPosition.x = Mathf.Clamp(newPosition.x, _border.BottomX, _border.TopX);
            newPosition.z = Mathf.Clamp(newPosition.z, _border.BottomY, _border.TopY);

            CameraSystem.transform.position = Vector3.Lerp(CameraSystem.transform.position, newPosition, speed);
            _subjectCameraMoving.OnNext(Unit.Default); //Камера сдвинулась, оповещаем
        }

        public Vector2 GetWorldPoint(Vector2 mousePosition)
        {
            var ray = Camera.main.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, 0));
            Vector3 normal = Vector3.up;
            Vector3 position = Vector3.zero;
            Plane plane = new Plane(normal, position);

            if (plane.Raycast(ray, out var distance))
            {
                var point = ray.GetPoint(distance);
                return new Vector2(point.x, point.z);
            }

            return new Vector2(0, 0);
        }

        public void AutoMoving()
        {
            if (!_autoMoving) return;

            CameraSystem.transform.position = Vector3.SmoothDamp(CameraSystem.transform.position, _targetAutoMoving,
                ref _velocity, smoothTime, speed);
            _subjectCameraMoving.OnNext(Unit.Default); //Камера сдвинулась, оповещаем
            if (_velocity.magnitude < 0.0005)
            {
                _autoMoving = false;
                CameraSystem.transform.position = _targetAutoMoving;
            }
        }

        public void MoveCamera(Vector2Int position)
        {
            _autoMoving = true;
            var t = 2;
            _targetAutoMoving = new Vector3(
                position.x + t,
                CameraSystem.transform.position.y,
                position.y + t
            );
        }
    }
}