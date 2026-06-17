using Game.Settings.Gameplay.Entities.Heroes;
using Game.Settings.Gameplay.Entities.Tower;
using MVVM.CMD;

namespace Game.GameRoot.Queries.HeroQueries
{
    public class QueryInfoHero : IQuery<HeroSettings>
    {
        public readonly string ConfigId;

        public QueryInfoHero(string configId)
        {
            ConfigId = configId;
        }
    }
    
}