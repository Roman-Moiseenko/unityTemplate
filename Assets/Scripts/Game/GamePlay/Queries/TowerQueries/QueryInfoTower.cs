using Game.Settings.Gameplay.Entities.Tower;
using MVVM.CMD;

namespace Game.GamePlay.Queries.TowerQueries
{
    public class QueryInfoTower : IQuery<TowerSettings>
    {
        public string ConfigId;
        public int Level; //???
    }
}