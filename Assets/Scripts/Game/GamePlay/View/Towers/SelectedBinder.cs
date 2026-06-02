using UnityEngine;

namespace Game.GamePlay.View.Towers
{
    public class SelectedBinder : MonoBehaviour
    {
        [SerializeField] private Transform plane; 
        
        private static readonly int UnscaledTime = Shader.PropertyToID("_UnscaledTime");
        private Material _material;
        private bool _isEnable;
        
        private void Awake()
        {
            if (plane != null)
                _material = plane.GetComponent<Renderer>().material;
        }

        private void OnEnable()
        {
            _isEnable = true;
        }
        
        private void OnDisable()
        {
            _isEnable = false;
        }

        private void Update()
        {
            if (_isEnable)
            {
                _material.SetFloat(UnscaledTime, Time.unscaledTime);
            }
        }
        
        private void OnDestroy()
        {
            if (_material != null)
                Destroy(_material);
        }

    }
}
