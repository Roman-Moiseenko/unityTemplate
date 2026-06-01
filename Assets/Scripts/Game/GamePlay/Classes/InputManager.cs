using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Game.GamePlay.Classes
{
    public class InputManager : MonoBehaviour
    {
        private PlayerInputActions _playerControls;
        private Vector2 _currentPointerPosition;
        private Vector2 _currentSecondPosition;
        private Vector2 _tapStartPosition;
        private Vector2 _tapStartSecondPosition = Vector2.zero;
        private bool _isPointerDown;
        private bool _isDragActive;
        private float _pressStartTime;
        private const float TapHoldThreshold = 0.2f;
        private float _tapThresholdDistance = 10f;

        public static event System.Action<Vector2> OnTapPerformed;
        public static event System.Action<Vector2> OnPointerDown;
        public static event System.Action<Vector2> OnPointerUp;
        public static event System.Action<Vector2, Vector2> OnPointerDrag;
        public static event System.Action<Vector2> OnTapUI;
        public static event System.Action<bool> OnScalingUp;

        private Camera mainCamera;
        private bool _isScrolling = false;

        private void Awake()
        {
            _playerControls = new PlayerInputActions();
            mainCamera = Camera.main;

            _playerControls.Gameplay.Click.started += OnPointerStarted;
            _playerControls.Gameplay.Click.performed += OnClick;
            _playerControls.Gameplay.Click.canceled += OnPointerReleased;
            _playerControls.Gameplay.Move.performed += ReadPointerPosition;
            _playerControls.Gameplay.Move.canceled += ReadPointerPosition;
            _playerControls.UI.ClickUI.performed += OnClickUI;

            _playerControls.Gameplay.SecondaryContact.started += OnSecondaryContactStarted;
            _playerControls.Gameplay.SecondaryContact.canceled += OnSecondaryContactCanceled;
            _playerControls.Gameplay.SecondaryMove.performed += OnSecondaryContactPosition;
            _playerControls.Gameplay.ScrollMouse.performed += OnScrollWheel;

#if UNITY_EDITOR
            _tapThresholdDistance = 20f;
#elif UNITY_IOS || UNITY_ANDROID
            _tapThresholdDistance = 100f;
#endif
        }

        private void Update()
        {
            if (_isPointerDown && !_isDragActive)
            {
                float pressDuration = Time.unscaledTime - _pressStartTime;
                if (pressDuration >= TapHoldThreshold)
                {
                    _isDragActive = true;
                    OnPointerDown?.Invoke(_tapStartPosition);
                }
            }
        }

        private void OnScrollWheel(InputAction.CallbackContext context)
        {
            var scrollWheelValue = context.ReadValue<Vector2>().y;
            if (scrollWheelValue != 0) OnScalingUp?.Invoke(scrollWheelValue > 0);
        }

        private void OnSecondaryContactPosition(InputAction.CallbackContext context)
        {
            _currentSecondPosition = context.ReadValue<Vector2>();
            if (IsPointerOverUIObject(_currentSecondPosition)) return;
            if (_tapStartSecondPosition == Vector2.zero) _tapStartSecondPosition = _currentSecondPosition;
            Vector2 delta = _currentSecondPosition - _tapStartSecondPosition;
            if (delta.magnitude > _tapThresholdDistance)
            {
                var distBegin = Vector2.Distance(_currentPointerPosition, _tapStartSecondPosition);
                var distEnd = Vector2.Distance(_currentPointerPosition, _currentSecondPosition);
                OnScalingUp?.Invoke(distBegin < distEnd);
                _tapStartSecondPosition = _currentSecondPosition;
            }
        }

        private void OnSecondaryContactStarted(InputAction.CallbackContext context)
        {
            if (_playerControls.Gameplay.Click.IsPressed()) _isScrolling = true;
        }

        private void OnSecondaryContactCanceled(InputAction.CallbackContext context) { _isScrolling = false; }

        private void OnClick(InputAction.CallbackContext context) { }

        private void OnClickUI(InputAction.CallbackContext context) { OnTapUI?.Invoke(_currentPointerPosition); }

        private void ReadPointerPosition(InputAction.CallbackContext context)
        {
            _currentPointerPosition = context.ReadValue<Vector2>();
            if (!_isPointerDown) return;
            if (IsPointerOverUIObject(_currentPointerPosition)) return;

            Vector2 delta = _currentPointerPosition - _tapStartPosition;
            if (delta.magnitude > _tapThresholdDistance)
            {
                if (!_isDragActive)
                {
                    _isDragActive = true;
                    OnPointerDown?.Invoke(_tapStartPosition);
                }
                if (_isScrolling)
                {
                    var distBegin = Vector2.Distance(_currentSecondPosition, _tapStartPosition);
                    var distEnd = Vector2.Distance(_currentSecondPosition, _currentPointerPosition);
                    OnScalingUp?.Invoke(distBegin < distEnd);
                }
                else
                {
                    OnPointerDrag?.Invoke(_tapStartPosition, _currentPointerPosition);
                }
                _tapStartPosition = _currentPointerPosition;
            }
        }

        private void OnPointerReleased(InputAction.CallbackContext context)
        {
            _isPointerDown = false;
            Vector2 releasePosition = _currentPointerPosition;
            float movedDistance = Vector2.Distance(_tapStartPosition, releasePosition);
            float pressDuration = Time.unscaledTime - _pressStartTime;

            if (_isDragActive)
            {
                _isDragActive = false;
                OnPointerUp?.Invoke(releasePosition);
            }
            else if (movedDistance < _tapThresholdDistance && pressDuration < TapHoldThreshold)
            {
                OnTapPerformed?.Invoke(releasePosition);
            }
        }

        private void OnPointerStarted(InputAction.CallbackContext context)
        {
            if (IsPointerOverUIObject(_currentPointerPosition)) return;
            _isPointerDown = true;
            _isDragActive = false;
            _tapStartPosition = _currentPointerPosition;
            _pressStartTime = Time.unscaledTime;
        }

        private void OnEnable() { _playerControls.Enable(); }
        private void OnDisable() { _playerControls.Disable(); }

        public Vector3 ScreenToWorld(Vector2 screenPosition)
        {
            if (mainCamera == null) return Vector3.zero;
            return mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, mainCamera.nearClipPlane));
        }

        public Vector2 ScreenToCanvas(Vector2 screenPosition, RectTransform canvasRect)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, null, out Vector2 pos);
            return pos;
        }

        private bool IsPointerOverUIObject(Vector2 screenPosition)
        {
            if (EventSystem.current == null) return false;
            var eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = screenPosition;
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            foreach (var result in results)
            {
                if (result.gameObject.layer == LayerMask.NameToLayer("UI")) return true;
            }
            return false;
        }
    }
}