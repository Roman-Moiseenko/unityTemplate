using System;
using System.Collections.Generic;
using Game.GamePlay.Classes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Game.GamePlay.Controllers
{
    [RequireComponent(typeof(GameObject))]
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform cameraSystem;
        
        [Range(0, 20f)] public float moveSpeed = 10f;
        [Range(0f, 10f)] public float sensitivity = 1;
        private float _sensTouch = 0.1f;

        private float _tempSens;
        private bool _isDragging, _isMoving;
        private Vector2 _tempCenter, _targetDirection, _tempMousePos;

        private RectBorder _border, _cameraBorder;

        private bool _isAutoMoving = false;

        private Vector3 _targetAutoMoving;
        private void Start()
        {
            _camera = GetComponent<Camera>();
            _isDragging = false;
            _isMoving = false;
            //Разная чувствительность для Редактора и Телефона
#if UNITY_EDITOR
            moveSpeed = 4f;
            sensitivity = 1.0f;
            _sensTouch = 0.1f;
#elif UNITY_IOS || UNITY_ANDROID
        moveSpeed = 4f;
        sensitivity = 1.7f;
        sens_touch = 0.4f;
#endif
            //Определяем границы хода камеры
            float _centreX = 4;
            float _centreY = 4;
            float width = 8;
            float height = 8;
            //Данные по камере
            //  Vector2 offset = _camera.ScreenToWorldPoint(new Vector2(0, 0));
            //Debug.Log($"Camera X {offset.x} Y {offset.y}");
            _border.BottomX = _centreX - width / 2; // - offset.x;
            _border.BottomY = _centreY - height / 2; // - offset.y;
            _border.TopX = _centreX + width / 2; // + offset.x;
            _border.TopY = _centreY + height / 2; // + offset.y;

            SetPositionCamera(_centreX, _centreY);
        }

        private void SetPositionCamera(float x, float y)
        {
            float _newPosX = Mathf.Clamp(x, _border.BottomX, _border.TopX);
            float _newPosY = Mathf.Clamp(y, _border.BottomY, _border.TopY);
           transform.position = new Vector3(_newPosX, transform.position.y, _newPosY);
//            Debug.Log(_newPosX + " " + transform.position.y  + " " + _newPosY);
        }
        
        private void Update()
        {
            UpdateInput();
            UpdatePosition();
            AutoMoving();
        }

        private void AutoMoving()
        {
            if (_isAutoMoving)
            {
                //Debug.Log(JsonUtility.ToJson(transform.position));
                //Debug.Log(JsonUtility.ToJson(targetAutoMoving));
                transform.position = Vector3.Lerp(transform.position, _targetAutoMoving, Time.deltaTime * moveSpeed / 3);
                float _t = (transform.position - _targetAutoMoving).sqrMagnitude;
                if (_t < _sensTouch) _isAutoMoving = false;
            }
        }

        private void UpdateInput()
        {
#if UNITY_EDITOR
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Vector2 mousePosition = Input.mousePosition;
                if (Input.GetMouseButtonDown(0)) OnPointDown(mousePosition);
                else if (Input.GetMouseButtonUp(0)) OnPointUp(mousePosition);
                else if (Input.GetMouseButton(0)) OnPointMove(mousePosition);
            }

#elif UNITY_IOS || UNITY_ANDROID
        Touch _touch = Input.GetTouch(0);
        if (!IsPointerOverUIObject()) {
            if (Input.touchCount > 0)
            {
                
                Vector2 touchPosition = _touch.position;
                if (_touch.phase == TouchPhase.Began) OnPointDown(touchPosition);
                if (_touch.phase == TouchPhase.Moved) OnPointMove(touchPosition);
                if (_touch.phase == TouchPhase.Ended) OnPointUp(touchPosition);
                if (_touch.phase == TouchPhase.Stationary) isDragging = false;
                
                var hit = new RaycastHit();
                for (var i = 0; i < Input.touchCount; ++i) {
                    if (Input.GetTouch(i).phase == TouchPhase.Began) {
                        var ray = camera.ScreenPointToRay(Input.GetTouch(i).position);
                        if (Physics.Raycast(ray, out hit)) {
                            hit.transform.gameObject.SendMessage("OnMouseDown");
                        }
                    }
                }
            }
        }
        else
        {
            if (BlockPanel != TypeBlockPanelUI.None && _touch.phase == TouchPhase.Ended)
            {
                Messenger<TypeBlockPanelUI>.Broadcast(Events.TOUCH_SCREEN, BlockPanel);
                BlockPanel = TypeBlockPanelUI.None;
            }
        }
#endif
        }

        private bool IsPointerOverUIObject() //Проверка для Андроид - EventSystem.current.IsPointerOverGameObject()
        {
            if (Input.touchCount > 0)
            {
                Touch _touch = Input.GetTouch(0);
                var touchPosition = _touch.position;
                var eventData = new PointerEventData(EventSystem.current) { position = touchPosition };
                var results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventData, results);
                return results.Count > 0;
            }

            return false;
        }

        private void UpdatePosition()
        {
            if (!_isMoving) return;

            float speed = Time.deltaTime * moveSpeed;
            if (_isDragging)
            {
                _tempSens = sensitivity;
            }
            else if (_tempSens > _sensTouch)
            {
                _tempSens = Mathf.Lerp(_tempSens, 0f, speed / 5);
            }

            if (_tempSens <= _sensTouch) _isMoving = false;
            Vector3 newPosition = transform.position + new Vector3(_targetDirection.x, 0, _targetDirection.y) * _tempSens;
            newPosition.x = Mathf.Clamp(newPosition.x, _border.BottomX, _border.TopX);
            newPosition.z = Mathf.Clamp(newPosition.z, _border.BottomY, _border.TopY);
            
            transform.position = Vector3.Lerp(transform.position, newPosition, speed);
        }

        private void OnPointDown(Vector2 mousePosition)
        {
            _tempCenter = GetWorldPoint(mousePosition);
            _targetDirection = Vector2.zero;
            _tempMousePos = mousePosition;
            _isDragging = true;
            _isMoving = true;
        }

        private void OnPointMove(Vector2 mousePosition)
        {
            if (_isDragging)
            {
                Vector2 point = GetWorldPoint(mousePosition);
                float sqrDst = (_tempCenter - point).sqrMagnitude;
                if (sqrDst > _sensTouch)
                {
                    //targetDirection = mousePosition.normalized;
                    if (_tempMousePos != mousePosition)
                    {
                        var _targetDirection = (_tempMousePos - mousePosition).normalized;
                        this._targetDirection = RotateTarget(_targetDirection);
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

        private void OnPointUp(Vector2 mousePosition)
        {
            _isDragging = false;
            Vector2 point = GetWorldPoint(mousePosition);
            float sqrDst = (_tempCenter - point).sqrMagnitude;
            if (sqrDst <= _sensTouch) _isMoving = false;
        }

        //**** Вычисления
        private Vector2 GetWorldPoint(Vector2 mousePosition)
        {
            Vector3 point = _camera.ScreenToWorldPoint(mousePosition);
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