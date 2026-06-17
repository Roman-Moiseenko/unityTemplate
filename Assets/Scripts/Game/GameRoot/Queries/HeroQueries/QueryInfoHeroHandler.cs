using Game.Settings;
using Game.Settings.Gameplay.Entities.Heroes;
using MVVM.CMD;

namespace Game.GameRoot.Queries.HeroQueries
{
    public class QueryInfoHeroHandler : IQueryHandler<QueryInfoHero, HeroSettings>
    {
        public HeroSettings Handle(QueryInfoHero query, ISettingsProvider settingsProvider)
        {
            var gameSettings = settingsProvider.GameSettings;
            return gameSettings.HeroesSettings.AllHeroes.Find(t => t.ConfigId == query.ConfigId);
        }
    }
}