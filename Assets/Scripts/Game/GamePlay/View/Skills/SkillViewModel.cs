using System;
using System.Collections.Generic;
using Game.GamePlay.Services;
using Game.State.Common;
using Game.State.Gameplay;
using Game.State.Gameplay.Statistics;
using Game.State.Maps.Shots;
using Game.State.Maps.Skills;
using Game.State.Parameters;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Skills
{
    public class SkillViewModel : IDisposable
    {
        
        protected GameplayStateProxy GameplayState;
        private readonly SkillEntity _skillEntity;
        
        public string ConfigId => _skillEntity.ConfigId;
        public int UniqueId => _skillEntity.UniqueId;
        public readonly ReactiveProperty<int> Level;
        public readonly ReactiveProperty<bool> ToDestroy = new(false);
        public TypeEpic EpicLevel { get; set; }

        public ReactiveProperty<Vector2> EffectPosition = new(Vector2.zero);
        public ReactiveProperty<Vector2Int> EffectDirection = new(Vector2Int.zero);
        //  public FsmSkill FsmSkill;

        public Dictionary<ParameterType, ParameterData> Parameters => _skillEntity.Parameters;
        
        public readonly ReactiveProperty<bool> IsCooldown = new(false);
        public readonly ReactiveProperty<float> Cooldown = new(0f);

        public float TimeOut = 0f;

//        public readonly ReactiveProperty<bool> IsEnabled = new(true);
        public readonly ReactiveProperty<bool> IsActive = new(false);
        private readonly SkillsService _service;
        private DisposableBag _disposables;

        public SkillViewModel(SkillEntity skillEntity,
            SkillsService service, GameplayStateProxy gameplayState)
        {
            _service = service;
            GameplayState = gameplayState;
            _skillEntity = skillEntity;
            Level =  skillEntity.Level;
            //Время отката - подписываемся на изменение уровня для пересчёта Cooldown
            UpdateCooldown();
            Level.Subscribe(_ => UpdateCooldown()).AddTo(ref _disposables);
        }

        private void UpdateCooldown()
        {
            if (_skillEntity.Parameters.TryGetValue(ParameterType.Cooldown, out var parameterData))
                Cooldown.Value = parameterData.Value;
        }

        public void StartSkill()
        {
//            if (!IsEnabled.CurrentValue) return;
            if (TimeOut > 0) return;
            _service.StartSkill(ConfigId);
        }

        public void StartCooldown()
        {
            TimeOut = Cooldown.Value;
            IsCooldown.OnNext(true);
        }

        public void Dispose()
        {
            ToDestroy?.Dispose();
            Level?.Dispose();
            Cooldown?.Dispose();
            IsCooldown?.Dispose();
            IsActive?.Dispose();
            _disposables.Dispose();
        }

        public void SetDamageShot(int mobUniqueId, float damage)
        {
            var shotData = new ShotData
            {
                MobEntityId = mobUniqueId,
                ConfigId = ConfigId,
                Single = true,
                Damage = damage, 
                DamageType = DamageType.MassDamage,
                TypeEntity = TypeEntityStatisticDamage.Skill,
            };
            GameplayState.Shots.Add(shotData);
        }
    }
}