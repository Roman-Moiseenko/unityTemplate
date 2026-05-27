using Game.Settings;
using Game.Settings.Gameplay.Entities.Skill;
using MVVM.CMD;

namespace Game.GamePlay.Queries.SkillQueries
{
    public class QueryInfoSkillHandler : IQueryHandler<QueryInfoSkill, SkillSettings>
    {
        public SkillSettings Handle(QueryInfoSkill query, ISettingsProvider settingsProvider)
        {
            var gameSettings = settingsProvider.GameSettings;
            return gameSettings.SkillsSettings.AllSkills.Find(s => s.ConfigId == query.ConfigId);
        }
    }
}