using Game.Settings.Gameplay.Entities.Skill;
using MVVM.CMD;

namespace Game.GamePlay.Queries.SkillQueries
{
    public class QueryInfoSkill : IQuery<SkillSettings>
    {
        public string ConfigId;
    }
}