using System.Collections.Generic;
using System.Linq;
using Game.GamePlay.Queries.Classes;
using Game.Settings;
using MVVM.CMD;


namespace Game.GamePlay.Queries.WaveQueries
{
    public class QueryInfoWaveHandler : IQueryHandler<QueryInfoWave, List<EnemyDataInfo>>
    {
        private readonly int _mapId;

        public QueryInfoWaveHandler(int mapId)
        {
            _mapId = mapId;
        }
        public List<EnemyDataInfo> Handle(QueryInfoWave query, ISettingsProvider settingsProvider)
        {
            var gameSettings = settingsProvider.GameSettings;
            var newMapSettings = gameSettings.MapsSettings.Maps.First(m => m.MapId == _mapId).InitialStateSettings;

            //Список мобов по основному или второму пути
            var settingsWave = query.IsWay 
                ? newMapSettings.Waves[query.NumberWave - 1] 
                : newMapSettings.WavesSecond[query.NumberWave - 1];
            
            var mobsSettings = gameSettings.MobsSettings;
            List<EnemyDataInfo> result = new();
            
            foreach (var waveItemSettings in settingsWave.WaveItems)
            {
                var enemy = result.Find(e => e.ConfigId == waveItemSettings.MobConfigId);
                if (enemy == null)
                {
                    var newEnemy = new EnemyDataInfo
                    {
                        Quantity = waveItemSettings.Quantity,
                        ConfigId = waveItemSettings.MobConfigId,
                    };
                    
                    var mobSettings = mobsSettings.AllMobs.Find(t => t.ConfigId == newEnemy.ConfigId);
                    if (mobSettings == null)
                    {
                        mobSettings = mobsSettings.AllBosses.Find(t => t.ConfigId == newEnemy.ConfigId);
                        newEnemy.IsBoss = true;
                    }

                    newEnemy.Defence = mobSettings.Defence;
                    newEnemy.TitleLid = mobSettings.TitleLid;
                    result.Add(newEnemy);
                }
                else
                {
                    enemy.Quantity += waveItemSettings.Quantity;
                }
                
            }
            return result;
        }
    }
}