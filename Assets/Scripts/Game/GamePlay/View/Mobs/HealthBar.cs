using System.Collections.Generic;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Mobs
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Transform _health;
        [SerializeField] private Transform _level;
        [SerializeField] private List<Texture2D> _sprites;
        
        private Camera _camera;
        private float _maxHealth = 1f;
        private float _currentHealth = 1f;
        private Material _material;
        public void Bind(Camera camera, float maxHealth, ReactiveProperty<float> currentHealth, int level)
        {
            _camera = camera;
            _maxHealth = maxHealth;
            _currentHealth = maxHealth;
            _material = _health.GetComponent<Renderer>().material;
            var matBlock = new MaterialPropertyBlock();
            currentHealth.Subscribe(h => _currentHealth = h);

            var first = level / 10;
            var second = level % 10;
            
            var meshLevel = _level.GetComponent<MeshRenderer>();
            meshLevel.GetPropertyBlock(matBlock);
            matBlock.SetFloat("_First", first);
            matBlock.SetFloat("_Second", second);
            meshLevel.SetPropertyBlock(matBlock);
        }

        public void OnUpdate()
        {
            if (_currentHealth < _maxHealth)
            {
                gameObject.SetActive(true);
                UpdateParams();
            }
            else
            {
                gameObject.SetActive(false);
            }
            AlignCamera();
        }

        /**
 * Поворачиваем полоску к камере
 */
        private void AlignCamera() {
            if (_camera != null) {
                var camXform = _camera.transform;
                var forward = transform.position - camXform.position;
                forward.Normalize();
                var up = Vector3.Cross(forward, camXform.right);
                transform.rotation = Quaternion.LookRotation(forward, up);
            }
        }
        
        /**
         * Обновляем шрейдер
         */
        private void UpdateParams() {
            _material.SetFloat("_Fill", _currentHealth / _maxHealth);
        }
        
    }
}