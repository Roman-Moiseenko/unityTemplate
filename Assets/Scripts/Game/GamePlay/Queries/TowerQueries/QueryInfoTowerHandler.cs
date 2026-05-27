using Game.Settings;
using Game.Settings.Gameplay.Entities.Tower;
using Game.State.Gameplay;
using Game.State.Root;
using MVVM.CMD;

namespace Game.GamePlay.Queries.TowerQueries
{
    public class QueryInfoTowerHandler : IQueryHandler<QueryInfoTower, TowerSettings>
    {

        public TowerSettings Handle(QueryInfoTower query, ISettingsProvider settingsProvider)
        {
            var gameSettings = settingsProvider.GameSettings;
            var towerSettings = gameSettings.TowersSettings.AllTowers.Find(t => t.ConfigId == query.ConfigId);
            //TODO Переделать под ViewModel и вытащить данные по уровню

            return towerSettings;
        }
    }
}