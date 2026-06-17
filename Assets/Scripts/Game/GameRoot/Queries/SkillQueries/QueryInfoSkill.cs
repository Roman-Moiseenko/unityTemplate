using Game.Settings.Gameplay.Entities.Skill;
using MVVM.CMD;

namespace Game.GameRoot.Queries.SkillQueries
{
    public class QueryInfoSkill : IQuery<SkillSettings>
    {
        public readonly string ConfigId;

        public QueryInfoSkill(string configId)
        {
            ConfigId = configId;
        }
    }
}