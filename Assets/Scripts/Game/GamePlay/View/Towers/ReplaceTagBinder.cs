using UnityEngine;

namespace Game.GamePlay.View.Towers
{
    using UnityEngine;

    namespace Game.GamePlay.View.Towers
    {
        public class ReplaceTagBinder : MonoBehaviour
        {
            [Header("Bobbing Settings")] [SerializeField]
            private float _bobSpeed = 2f;

            [SerializeField] private float _bobHeight = 0.15f;
            [SerializeField] private float _rotationAngle = 5f;
            [SerializeField] private float _rotationSpeed = 1.5f;
            [SerializeField] private bool _randomizeOffset = true;

            private Vector3 _initialPosition;
            private Quaternion _initialRotation;
            private float _phaseOffset;

            private void Awake()
            {
                _initialPosition = transform.localPosition;
                _initialRotation = transform.localRotation;

                if (_randomizeOffset)
                    _phaseOffset = Random.Range(0f, Mathf.PI * 2f);
            }

            private void OnEnable()
            {
                ResetTransform();
            }

            private void OnDisable()
            {
                ResetTransform();
            }

            private void Update()
            {
                float time = Time.unscaledTime;

                // Vertical bobbing (покачивание вверх-вниз)
                float yOffset = Mathf.Sin(time * _bobSpeed + _phaseOffset) * _bobHeight;
                Vector3 targetPosition = _initialPosition + new Vector3(0f, yOffset, 0f);
                transform.localPosition = targetPosition;

                // Gentle rotation (легкое покачивание поворотом)
                float zRotation = Mathf.Sin(time * _rotationSpeed + _phaseOffset + 1.57f) * _rotationAngle;
                transform.localRotation = _initialRotation * Quaternion.Euler(0f, 0f, zRotation);
            }

            private void ResetTransform()
            {
                transform.localPosition = _initialPosition;
                transform.localRotation = _initialRotation;
            }

            private void OnDrawGizmosSelected()
            {
                if (!Application.isPlaying)
                {
                    _initialPosition = transform.localPosition;
                    _initialRotation = transform.localRotation;
                }
            }

#if UNITY_EDITOR
            private void OnValidate()
            {
                _bobSpeed = Mathf.Max(0f, _bobSpeed);
                _bobHeight = Mathf.Max(0f, _bobHeight);
                _rotationAngle = Mathf.Clamp(_rotationAngle, 0f, 45f);
                _rotationSpeed = Mathf.Max(0f, _rotationSpeed);
            }
#endif
        }
    }
}
