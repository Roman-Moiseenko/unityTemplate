using System.Collections.Generic;
using System.Linq;
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
            var settingsWaves = newMapSettings.InitialStateSettings.Waves;
            var items = settingsWaves[query.NumberWave - 1];

            Dictionary<string, int> info = new();
            
            foreach (var item in items.WaveItems)
            {
                if (info.TryGetValue(item.MobConfigId, out var countItem))
                {
                    info[item.MobConfigId] = countItem + item.Quantity;
                }
                else
                {
                    info.Add(item.MobConfigId, item.Quantity);
                }
            }
            return info;
        }
    }
}