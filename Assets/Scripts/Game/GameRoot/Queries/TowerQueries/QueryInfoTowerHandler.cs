using Game.Settings;
using Game.Settings.Gameplay.Entities.Tower;
using MVVM.CMD;

namespace Game.GameRoot.Queries.TowerQueries
{
    public class QueryInfoTowerHandler : IQueryHandler<QueryInfoTower, TowerSettings>
    {

        public TowerSettings Handle(QueryInfoTower query, ISettingsProvider settingsProvider)
        {
            var gameSettings = settingsProvider.GameSettings;
            var towerSettings = gameSettings.TowersSettings.AllTowers.Find(t => t.ConfigId == query.ConfigId);

            return towerSettings;
        }
    }
}