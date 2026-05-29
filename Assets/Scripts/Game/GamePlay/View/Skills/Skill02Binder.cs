using System;
using Game.State.Common;
using Game.State.Maps.Skills;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Skills
{
    public class Skill02Binder : SkillBinder, IHasHeathViewModel
    {
        [SerializeField] private Transform bar;
        [SerializeField] private Renderer barRenderer;

        private Camera _camera;
        private Material _barMaterial;
        private float _health;
        private float _duration;

        private float _maxDuration;
        private float _maxHealth;
        private bool _initialized;

        public int UniqueId { get; set; }
        public ReadOnlyReactiveProperty<bool> IsDead => _isDead;
        private readonly ReactiveProperty<bool> _isDead = new(false);
        
        public float Duration => _duration;
        protected override void OnBind()
        {
            UniqueId = -1; //Костыль. Навыки не имеют UniqueId, для стены ставим значение, которое не может быть у других, это < 0
            _camera = Camera.main;

            // Поворот эффекта по направлению
            var vector = new Vector3(ViewModel.EffectDirection.Value.x, 0, ViewModel.EffectDirection.Value.y);
            transform.rotation = Quaternion.LookRotation(vector);

            if (ViewModel.Parameters.TryGetValue(SkillParameterType.Health, out var paramHealth))
            {
                _maxHealth = paramHealth.Value;
                _health = _maxHealth;
            }
            else
            {
                throw new Exception("Отсутствует параметр здоровье");
            }

            if (ViewModel.Parameters.TryGetValue(SkillParameterType.Duration, out var paramDuration))
            {
                _maxDuration = paramDuration.Value;
                _duration = _maxDuration;
            }
            else
            {
                throw new Exception("Отсутствует параметр длительность");
            }

            // Создаём копию материала для независимого управления параметрами
            if (barRenderer != null)
            {
                _barMaterial = barRenderer.material;
            }

            _initialized = true;
            UpdateShaderProperties();
        }

        private void Update()
        {
            if (!_initialized) return;

            // Уменьшаем длительность (здоровье будет уменьшаться позже извне)
            if (_duration > 0)
            {
                _duration -= Time.deltaTime;
                if (_duration < 0) _duration = 0;
                UpdateShaderProperties();
            }

            // Проверка на уничтожение
            if (_duration <= 0 || _health <= 0)
            {
                _isDead.Value = true;
                ViewModel.ToDestroy.Value = true;
            }

            AlignCamera();
        }

        /// <summary>
        /// Устанавливает текущее значение здоровья (вызывается извне, например, при получении урона)
        /// </summary>
        public void SetHealth(float value)
        {
            _health = Mathf.Max(0, value);
            UpdateShaderProperties();
        }

        private void UpdateShaderProperties()
        {
            if (_barMaterial == null) return;

            _barMaterial.SetFloat("_FillDuration", _maxDuration > 0 ? _duration / _maxDuration : 0);
            _barMaterial.SetFloat("_FillHealth", _maxHealth > 0 ? _health / _maxHealth : 0);
        }

        private void AlignCamera()
        {
            if (_camera != null && bar != null)
            {
                var camXform = _camera.transform;
                var forward = bar.position - camXform.position;
                forward.Normalize();
                var up = Vector3.Cross(forward, camXform.right);
                bar.rotation = Quaternion.LookRotation(forward, up);
            }
        }

        protected override void OnDestroy()
        {
            // Уничтожаем созданную копию материала
            if (_barMaterial != null)
            {
                Destroy(_barMaterial);
            }
            base.OnDestroy();
        }


        public void DamageReceived(float damage, TypeDefence defence)
        {
            _health -= damage;
        }
    }
}
