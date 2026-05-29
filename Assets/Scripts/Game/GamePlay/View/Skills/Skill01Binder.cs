using System.Collections;
using System.Collections.Generic;
using Game.GamePlay.View.Mobs;
using Game.State.Maps.Skills;
using UnityEngine;
using UnityEngine.VFX;

namespace Game.GamePlay.View.Skills
{
    public class Skill01Binder : SkillBinder
    {
        [SerializeField] private List<Skill01RocketBinder> rockets = new();
        [SerializeField] private VisualEffect bottomArea;
        
        private float _dps;
        private float _duration;
        private float _range;
        private float _radius;
        private Vector3 _position;

        protected override void OnBind()
        {
            _position = new Vector3(ViewModel.EffectPosition.Value.x, 0, ViewModel.EffectPosition.Value.y);

            if (ViewModel.Parameters.TryGetValue(SkillParameterType.DPS, out var paramDPS))
                _dps = paramDPS.Value;
            if (ViewModel.Parameters.TryGetValue(SkillParameterType.Duration, out var paramDuration))
                _duration = paramDuration.Value;
            if (ViewModel.Parameters.TryGetValue(SkillParameterType.Range, out var paramRange))
                _range = paramRange.Value;

            _radius = _range / 2f;
            bottomArea.SetFloat("Radius", _radius);
            
            foreach (var rocket in rockets)
            {
                rocket.Bind();
            }
            
            StartCoroutine(DamageCoroutine());
            StartCoroutine(RocketsCoroutine());
        }

        private IEnumerator RocketsCoroutine()
        {
            // Время полёта одной ракеты от Y=15 до Y=0
            float flyTime = Skill01RocketBinder.StartY / Skill01RocketBinder.Speed; // 15 / 50 = 0.3с
            // Задержка между стартами ракет, чтобы равномерно распределить их в полёте
            float delayBetweenRockets = flyTime / rockets.Count; // 0.3 / 3 = 0.1с

            // Запускаем каждую ракету с задержкой
            for (int i = 0; i < rockets.Count; i++)
            {
                rockets[i].StartLoop();
                yield return new WaitForSeconds(delayBetweenRockets);
            }

            // После того как все ракеты запущены, они работают autonomously,
            // каждая сама летит вниз, взрывается, возвращается и повторяет цикл.
        }

        private IEnumerator DamageCoroutine()
        {
            // Первый тик — сразу
            ApplyDamageToMobsInRange();

            // С каждым тиком _duration уменьшается на 1
            // Не срабатывает когда _duration = 0
            while (_duration > 1f)
            {
                yield return new WaitForSeconds(1f);
                _duration -= 1f;
                ApplyDamageToMobsInRange();
            }

            // Ждём последнюю секунду без урона
            if (_duration > 0f)
            {
                yield return new WaitForSeconds(_duration);
            }

            ViewModel.ToDestroy.Value = true;
        }

        private void ApplyDamageToMobsInRange()
        {
            List<MobBinder> mobsInRange = new();

            Collider[] colliders = Physics.OverlapSphere(_position, _radius);
            foreach (var collider in colliders)
            {
                if (!collider.gameObject.CompareTag("Mob")) continue;

                var mobBinder = collider.gameObject.GetComponent<MobBinder>();
                if (mobBinder == null) continue;
                if (mobBinder.ViewModel.IsDead.CurrentValue) continue;

                mobsInRange.Add(mobBinder);
            }

            // Заглушка: нанести урон всем мобам из списка mobsInRange
            ApplyDamageToMobs(mobsInRange);
        }

        private void ApplyDamageToMobs(List<MobBinder> mobs)
        {
            // TODO: нанести урон _dps каждому мобу
            foreach (var mob in mobs)
            {
                ViewModel.SetDamageShot(mob.UniqueId, _dps);
              //  mob.ViewModel.SetDamage(_dps);
                //Debug.Log("Моб =  " + mob.UniqueId);
            }
        }
    }
}