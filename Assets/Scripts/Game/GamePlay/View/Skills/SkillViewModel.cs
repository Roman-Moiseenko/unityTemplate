using System;
using Game.GamePlay.Services;
using Game.State.Common;
using Game.State.Maps.Skills;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Skills
{
    public class SkillViewModel : IDisposable
    {
        private readonly SkillEntity _skillEntity;
        public string ConfigId => _skillEntity.ConfigId;
        public int UniqueId => _skillEntity.UniqueId;
        public readonly ReactiveProperty<int> Level;
        public readonly ReactiveProperty<bool> ToDestroy = new(false);
        public TypeEpic EpicLevel { get; set; }

        public ReactiveProperty<Vector2Int> EffectPosition = new(Vector2Int.zero);
        public ReactiveProperty<Vector2Int> EffectDirection = new(Vector2Int.zero);
        //  public FsmSkill FsmSkill;

        public readonly ReactiveProperty<bool> IsCooldown = new(false);
        public readonly float Cooldown = 0f;

        public float TimeOut = 0f;

//        public readonly ReactiveProperty<bool> IsEnabled = new(true);
        public readonly ReactiveProperty<bool> IsActive = new(false);
        private readonly SkillsService _service;

        public SkillViewModel(SkillEntity skillEntity,
            SkillsService service)
        {
            _service = service;
            _skillEntity = skillEntity;
            Level =  skillEntity.Level;
            //Время отката
//            Debug.Log(JsonConvert.SerializeObject(skillEntity.Parameters, Formatting.Indented));
            if (skillEntity.Parameters.TryGetValue(SkillParameterType.Cooldown, out var parameterData)) 
                Cooldown = parameterData.Value;
        }

        public void StartSkill()
        {
//            if (!IsEnabled.CurrentValue) return;
            if (TimeOut > 0) return;
            _service.StartSkill(ConfigId);
        }

        public void StartCooldown()
        {
            TimeOut = Cooldown;
            IsCooldown.OnNext(true);
        }

        public void Dispose()
        {
            ToDestroy?.Dispose();
            Level?.Dispose();
            IsCooldown?.Dispose();
            IsActive?.Dispose();
        }
    }
}