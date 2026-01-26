using System.Collections.Generic;
using System.Linq;
using Game.GamePlay.Queries.Classes;
using Game.Settings;
using Game.State.Root;
using MVVM.CMD;

namespace Game.GamePlay.Queries.WaveQueries
{
    public class QueryInfoWaveHandler : IQueryHandler<QueryInfoWave>
    {
        private readonly int _mapId;

        public QueryInfoWaveHandler(GameplayStateProxy gameplayState)
        {
            _mapId = gameplayState.MapId.CurrentValue;
        }
        public object Handle(QueryInfoWave query, ISettingsProvider settingsProvider)
        {
            var gameSettings = settingsProvider.GameSettings;
            var newMapSettings = gameSettings.MapsSettings.Maps.First(m => m.MapId == _mapId);
            var settingsWave = newMapSettings.InitialStateSettings.Waves[query.NumberWave - 1];
            //Список мобов по основному или второму пути
            var waveItems = query.IsWay ? settingsWave.WaveItems : settingsWave.WaveSecondItems;
            var mobsSettings = gameSettings.MobsSettings;
            List<EnemyDataInfo> result = new();
            
            foreach (var waveItemSettings in waveItems)
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