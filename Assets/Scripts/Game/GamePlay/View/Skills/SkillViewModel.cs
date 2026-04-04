using DI;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.SkillStates;
using Game.State.Common;
using Game.State.Maps.Skills;
using R3;

namespace Game.GamePlay.View.Skills
{
    public class SkillViewModel
    {
        private readonly SkillEntity _skillEntity;
        public string ConfigId => _skillEntity.ConfigId;
        public int UniqueId => _skillEntity.UniqueId;
        public ReactiveProperty<int> Level { get; set; }
        public TypeEpic EpicLevel { get; set; }
        public FsmSkill FsmSkill;

        public float Cooldown = 0f;

        public float TimeOut = 0;
        
        public SkillViewModel(SkillEntity skillEntity,
            DIContainer container)
        {
            _skillEntity = skillEntity;

            //Время отката
            if (skillEntity.Parameters.TryGetValue(SkillParameterType.Cooldown, out var parameterData)) 
                Cooldown = parameterData.Value;
            
            FsmSkill = container.Resolve<FsmSkill>();
            
            //TODO Подписка на FsmSkill Запуска TimeOut
            
        }

        public void StartSkill()
        {
            FsmSkill.Fsm.SetState<FsmSkillBegin>(ConfigId);
        }
    }
}