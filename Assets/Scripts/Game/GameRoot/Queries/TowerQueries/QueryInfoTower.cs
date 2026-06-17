using Game.Settings.Gameplay.Entities.Tower;
using MVVM.CMD;

namespace Game.GameRoot.Queries.TowerQueries
{
    public class QueryInfoTower : IQuery<TowerSettings>
    {
        public string ConfigId;
    }
}