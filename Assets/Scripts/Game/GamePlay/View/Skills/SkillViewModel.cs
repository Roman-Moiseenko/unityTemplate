using DI;
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
        
        public SkillViewModel(SkillEntity skillEntity,
            DIContainer container)
        {
            _skillEntity = skillEntity;
            
            
            
            
        }
    }
}