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

        public float Cooldown = 0f;

        public readonly ReactiveProperty<float> TimeOut = new(0f);

        public readonly ReactiveProperty<bool> IsEnabled = new(true);
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
            _service.StartSkill(ConfigId);

        }

        public void StartCooldown()
        {
            Debug.Log(Cooldown + " StartCooldown");
            TimeOut.OnNext(Cooldown);
        }
    }
}