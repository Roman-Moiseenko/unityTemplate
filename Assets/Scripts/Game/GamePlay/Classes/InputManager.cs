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
        private Vector2 _currentPointerPosition; // Текущая позиция указателя (мышь или касание)
        private Vector2 _tapStartPosition;       // Позиция, где началось касание
        private bool _isPointerDown;             // Флаг, указывающий, нажат ли указатель
        private const float TapHoldThreshold = 0.2f; // Время в секундах, после которого нажатие считается "долгим"
        private const float TapThresholdDistance = 20f; // Расстояние в пикселях, после которого нажатие считается "драгом"
        public static event System.Action<Vector2> OnTapPerformed;        // Одинарный клик/короткое касание
        public static event System.Action<Vector2> OnPointerDown;         // Начало нажатия
        public static event System.Action<Vector2> OnPointerUp;           // Отпускание нажатия
        public static event System.Action<Vector2, Vector2> OnPointerDrag; // Перемещение нажатого указателя

        private Camera mainCamera; // Главная камера для преобразования ScreenToWorldPoint
        
        private void Awake()
        {
            _playerControls = new PlayerInputActions();
            mainCamera = Camera.main;
            _playerControls.Gameplay.Click.performed += OnClick;
            _playerControls.Gameplay.Click.started += OnPointerStarted;
            _playerControls.Gameplay.Click.canceled += OnPointerCanceled;
            _playerControls.Gameplay.Move.performed += ReadPointerPosition;
            _playerControls.Gameplay.Move.canceled += ReadPointerPosition;
        }

        private void ReadPointerPosition(InputAction.CallbackContext context)
        {
            _currentPointerPosition = context.ReadValue<Vector2>();
            // !!! ГЛАВНОЕ ИЗМЕНЕНИЕ: Проверяем, если указатель нажат и находится над UI
            if (_isPointerDown && IsPointerOverUIObject(_currentPointerPosition) /*EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()*/)
            {
                // Если мы были нажаты над игровым объектом, но сейчас тащим над UI
                // Можно прервать состояние isPointerDown или просто игнорировать drag событие
                // Здесь мы просто игнорируем OnPointerDrag, если над UI.
                // Если нужно полное прерывание, можно вызвать OnPointerCanceled для очистки.
                return;
            }
            // Если указатель нажат и перемещается, это "драг"
            if (_isPointerDown)
            {
                Vector2 delta = _currentPointerPosition - _tapStartPosition;
                // Можно реализовать OnPointerDrag здесь или в Update,
                // в зависимости от того, как часто вам нужны обновления.
                // Для smooth drag лучше в Update или как отдельное событие.
                // Пока просто выведем в консоль для демонстрации
                // Debug.Log($"Dragging. Current pos: {currentPointerPosition}");
                OnPointerDrag?.Invoke(_tapStartPosition, _currentPointerPosition);
            }
        }

        private void OnPointerCanceled(InputAction.CallbackContext context)
        {
            _isPointerDown = false;
            Vector2 releasePosition = _currentPointerPosition; // Позиция отпускания
            float tapDuration = (float)context.duration;       // Длительность нажатия
            float movedDistance = Vector2.Distance(_tapStartPosition, releasePosition);

            // Оповещаем об отпускании нажатия
            //Debug.Log($"Pointer UP at screen position: {releasePosition}");
            OnPointerUp?.Invoke(releasePosition);
            
            // Если нажатие было коротким и без значительного перемещения, считаем это "тапом"
            if (tapDuration < TapHoldThreshold && movedDistance < TapThresholdDistance)
            {
                //Debug.Log($"--> This was a TAP at screen position: {releasePosition}");
                OnTapPerformed?.Invoke(releasePosition);
            }
            else if (movedDistance >= TapThresholdDistance)
            {
                //Debug.Log($"--> This was a DRAG from {_tapStartPosition} to {releasePosition}");
                // Событие OnPointerDrag будет вызываться каждый кадр, пока нажато
            }
            else
            {
                //Debug.Log($"--> This was a long press (HOLD) at screen position: {releasePosition}");
            }
        }

        private void OnPointerStarted(InputAction.CallbackContext context)
        {
            //_currentPointerPosition = _playerControls.Gameplay.PointerPosition.ReadValue<Vector2>();
            
            // !!! ГЛАВНОЕ ИЗМЕНЕНИЕ: Проверяем, находится ли курсор над UI
            if (IsPointerOverUIObject(_currentPointerPosition) /*EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()*/)
            {
                //Debug.Log("Pointer Down: OVER UI. Ignoring game objects.");
                return; // Игнорируем начало нажатия, если оно над UI
            }
            
            _isPointerDown = true;
            _tapStartPosition = _currentPointerPosition; // Запоминаем начальную позицию

            // Оповещаем о начале нажатия
            //Debug.Log($"Pointer DOWN at screen position: {_tapStartPosition}");
            OnPointerDown?.Invoke(_tapStartPosition);
        }

        private void OnEnable()
        {
            _playerControls.Enable(); // Включаем все Action Maps при активации объекта
        }

        private void OnDisable()
        {
            _playerControls.Disable(); // Отключаем все Action Maps при деактивации объекта
        }
        
        private void OnClick(InputAction.CallbackContext context)
        {
            // После отпускания кнопки, мы оцениваем, был ли это "тап" или "драг".
            // Это делается в OnPointerCanceled, чтобы учесть фактическое время и расстояние.
        //    Debug.Log("Клик" );
//            Debug.Log("Клик" );

        }
        
        public Vector3 ScreenToWorld(Vector2 screenPosition)
        {
            if (mainCamera == null)
            {
                //Debug.LogError("Main Camera not found. Please ensure your camera is tagged 'MainCamera'.");
                return Vector3.zero;
            }

            // Для 2D-сцены обычно z-координату задают 0 или на уровне игровой плоскости.
            // Для 3D-сцены нужно использовать Depth.
            return mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, mainCamera.nearClipPlane));
        }

        /// <summary>
        /// Преобразует координаты экрана (пиксели, левый нижний угол = 0,0)
        /// в координаты UI (RectTransform).
        /// </summary>
        public Vector2 ScreenToCanvas(Vector2 screenPosition, RectTransform canvasRect)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, null, out pos);
            return pos;
        }
        
        private bool IsPointerOverUIObject(Vector2 screenPosition)
        {
            if (EventSystem.current == null) return false;

            var eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = screenPosition;
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        
            // Проверяем, есть ли результаты Raycast и не является ли это только нашим собственным EventSystem,
            // а именно, есть ли активные UI элементы, перехватывающие клик.
            foreach (var result in results)
            {
                // Можно добавить более тонкую фильтрацию, если EventSystem сам по себе
                // иногда создает RaycastResult, который не является видимым UI элементом.
                if (result.gameObject.layer == LayerMask.NameToLayer("UI")) // Убедитесь, что UI элементы на слое "UI"
                {
                    return true;
                }
            }
            return false;
        }
    }
}