using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.ScreenGameplay
{
    public class SlantedSliderFill : MonoBehaviour
    {
        public Slider targetSlider;
        public Image fillImage; // Image компонент, на который назначен материал с шейдером
        [Range(-45f, 45f)]
        public float slantAngle = 15f;
        private Material _fillMaterial;
        void Start()
        {
            if (targetSlider == null)
            {
                Debug.LogError("Target Slider not assigned!", this);
                enabled = false;
                return;
            }
            if (fillImage == null || fillImage.material == null)
            {
                Debug.LogError("Fill Image or its Material not assigned!", this);
                enabled = false;
                return;
            }
            _fillMaterial = fillImage.material;
            // Убедитесь, что материал является инстансом, чтобы не изменять исходный ассет
            if (!_fillMaterial.name.EndsWith("(Instance)"))
            {
                _fillMaterial = new Material(fillImage.material);
                fillImage.material = _fillMaterial;
            }
            UpdateShader();
            targetSlider.onValueChanged.AddListener(delegate { UpdateShader(); });
        }
        void UpdateShader()
        {
            if (_fillMaterial != null)
            {
                _fillMaterial.SetFloat("_Progress", targetSlider.normalizedValue);
                _fillMaterial.SetFloat("_SlantAngle", slantAngle);
            }
        }
        void OnDestroy()
        {
            if (targetSlider != null)
            {
                targetSlider.onValueChanged.RemoveListener(delegate { UpdateShader(); });
            }
            if (_fillMaterial != null && _fillMaterial.name.EndsWith("(Instance)"))
            {
                Destroy(_fillMaterial); // Уничтожаем созданный инстанс материала
            }
        }
    }
}