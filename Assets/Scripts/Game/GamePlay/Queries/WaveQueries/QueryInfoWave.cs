using System.Collections.Generic;
using Game.GamePlay.Queries.Classes;
using MVVM.CMD;

namespace Game.GamePlay.Queries.WaveQueries
{
    public class QueryInfoWave : IQuery<List<EnemyDataInfo>>
    {
        public int NumberWave;
        public bool IsWay;
    }
}