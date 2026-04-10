using DI;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.SkillStates;
using Game.GamePlay.Services;
using Game.State.Common;
using Game.State.Maps.Skills;
using Newtonsoft.Json;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Skills
{
    public class SkillViewModel
    {
        private readonly SkillEntity _skillEntity;
        public string ConfigId => _skillEntity.ConfigId;
        public int UniqueId => _skillEntity.UniqueId;
        public ReactiveProperty<int> Level;
        public TypeEpic EpicLevel { get; set; }
      //  public FsmSkill FsmSkill;

        public ReactiveProperty<bool> IsCooldown = new(false);
        public float Cooldown = 0f;

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
            Debug.Log(JsonConvert.SerializeObject(skillEntity.Parameters, Formatting.Indented));
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
    }
}