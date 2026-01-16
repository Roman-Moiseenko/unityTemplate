using DG.Tweening;
using DI;
using Game.Common;
using Newtonsoft.Json;
using R3;
using UnityEngine;

namespace Game.GamePlay.Classes
{
    public class GameplayCamera
    {
        public Camera Camera;
        public Transform CameraSystem;

        public float MoveSpeed = 1f;
        public float Sensitivity = 5f;
        public float SensTouch = 0.4f;

        private const int speed = 8;
        private const float smoothTime = 0.5f;
        private Vector3 _velocity;


        private float _tempSens;
        private bool _isDragging = false; //, _isMoving = false;
        private Vector2 _tempCenter, _targetDirection, _tempMousePos;

        //private bool _autoMoving = false;
        private Vector3 _targetAutoMoving;

        private readonly RectBorder _border; //, _cameraBorder;
        private readonly Subject<Unit> _subjectCameraMoving;

        //private bool _moveTowards = false;

        public GameplayCamera(DIContainer container)
        {
            _subjectCameraMoving = container.Resolve<Subject<Unit>>(AppConstants.CAMERA_MOVING);
            CameraSystem = GameObject.Find("CameraSystem").GetComponent<Transform>();
            Camera = GameObject.Find("Main Camera").GetComponent<Camera>();

            int _centreX = AppConstants.CENTER_MAP; //Размеры игрового мира
            int _centreY = AppConstants.CENTER_MAP;
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
            //_isMoving = true;
        }

        public void OnPointMove(Vector2 mousePosition)
        {

            if (_isDragging)
            {
                Vector2 point = GetWorldPoint(mousePosition);
                float sqrDst = (_tempCenter - point).sqrMagnitude;
                //Debug.Log("sqrDst = " + sqrDst);
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
            //_autoMoving = false;
            Vector2 point = GetWorldPoint(mousePosition);
            float sqrDst = (_tempCenter - point).sqrMagnitude;
            // if (sqrDst <= SensTouch) _isMoving = false;
        }

        public void UpdateMoving()
        {
            if (!_isDragging) return;
            //if (!_isMoving) return;

            var speedMove = Time.unscaledDeltaTime * MoveSpeed;
            if (_isDragging)
            {
                _tempSens = Sensitivity;
//                Debug.Log("_tempSens 1 " + _tempSens);
            }
            else if (_tempSens > SensTouch)
            {
                _tempSens = Mathf.Lerp(_tempSens, 0f, speedMove / 5);
                Debug.Log("_tempSens 2 " + _tempSens + " " + SensTouch);
            }

            // if (_tempSens <= SensTouch) _isMoving = false;
            Vector3 newPosition = CameraSystem.transform.position +
                                  new Vector3(_targetDirection.x, 0, _targetDirection.y) * _tempSens;
            newPosition.x = Mathf.Clamp(newPosition.x, _border.BottomX, _border.TopX);
            newPosition.z = Mathf.Clamp(newPosition.z, _border.BottomY, _border.TopY);

            CameraSystem.transform.position = Vector3.Lerp(CameraSystem.transform.position, newPosition, speedMove);
            var dist = Vector3.Distance(CameraSystem.transform.position, newPosition);
//            Debug.Log("DIST = " + dist);
            _subjectCameraMoving.OnNext(Unit.Default); //Камера сдвинулась, оповещаем
           // if (dist is > 0 and < 1) _isDragging = false;
            
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
           
            /*if (!_autoMoving) return;

            if (_moveTowards) //Плавное движение
            {
                Debug.Log("AutoMoving 1");
                CameraSystem.transform.position = Vector3.MoveTowards(
                    CameraSystem.transform.position, 
                    _targetAutoMoving,
                    speed * Time.unscaledDeltaTime / 2.5f);
                var dist = Vector3.Distance(CameraSystem.transform.position, _targetAutoMoving);
                if (dist < 0.001)
                {
                    _autoMoving = false;
                    CameraSystem.transform.position = _targetAutoMoving;
                }
            }
            else //Рывками
            {
                Debug.Log("AutoMoving 2");
                CameraSystem.transform.position = Vector3.SmoothDamp(CameraSystem.transform.position, _targetAutoMoving,
                    ref _velocity, smoothTime, speed);

                if (_velocity.magnitude < 0.001)
                {
                    _autoMoving = false;
                    CameraSystem.transform.position = _targetAutoMoving;
                }
            }
            _subjectCameraMoving.OnNext(Unit.Default); //Камера сдвинулась, оповещаем
            */
        }

        public void MoveCamera(Vector2 position)
        {
            //_autoMoving = true;
            //_moveTowards = methods;
            _targetAutoMoving = new Vector3(
                position.x + AppConstants.CENTER_MAP,
                CameraSystem.transform.position.y,
                position.y + AppConstants.CENTER_MAP
            );
            
            //Debug.Log("MoveCamera " + _targetAutoMoving + " " + CameraSystem.transform.position);
            CameraSystem.transform
                .DOMove(_targetAutoMoving, smoothTime)
                .SetEase(Ease.OutCirc)
                .SetUpdate(true)
                .OnUpdate(() => _subjectCameraMoving.OnNext(Unit.Default));
        }
    }
}