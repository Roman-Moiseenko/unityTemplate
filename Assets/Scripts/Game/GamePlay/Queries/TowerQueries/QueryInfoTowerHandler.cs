using Game.Settings;
using Game.State.Root;
using MVVM.CMD;

namespace Game.GamePlay.Queries.TowerQueries
{
    public class QueryInfoTowerHandler : IQueryHandler<QueryInfoTower>
    {
        private readonly GameplayStateProxy _gameplayState;

        public QueryInfoTowerHandler(GameplayStateProxy gameplayState)
        {
            _gameplayState = gameplayState;
        }
        public object Handle(QueryInfoTower query, ISettingsProvider settingsProvider)
        {
            var gameSettings = settingsProvider.GameSettings;
            var towerSettings = gameSettings.TowersSettings.AllTowers.Find(t => t.ConfigId == query.ConfigId);
            //TODO Переделать под ViewModel и вытащить данные по уровню

            return towerSettings;
        }
    }
}