using System.Collections.Generic;
using DI;
using Game.State;
using Game.State.Gameplay;
using Game.State.Gameplay.Statistics;
using MVVM.UI;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.GamePlay.View.UI.PopupStatistics
{
    public class PopupStatisticsViewModel : WindowViewModel
    {
        public override string Id => "PopupStatistics";
        public override string Path => "Gameplay/Popups/";
        public GameplayStateProxy GameplayState;
        private StatisticGame _statisticGame ;
        public List<StatisticElementViewModel> Elements = new();
        public PopupStatisticsViewModel(DIContainer container) : base(container)
        {
            GameplayState = container.Resolve<IGameStateProvider>().GameplayState;
            _statisticGame = GameplayState.StatisticGame;

            Debug.Log(JsonConvert.SerializeObject(_statisticGame.Origin, Formatting.Indented));
            foreach (var pairStringFloat in _statisticGame.Damages)
            {
                var stat = new StatisticElementViewModel(container);
                stat.ConfigId = pairStringFloat.ConfigId;
                stat.Damage = pairStringFloat.Damage;
                stat.Percent = pairStringFloat.Damage / _statisticGame.AllDamage.CurrentValue * 100f;
                Elements.Add(stat);
            }
            
        }

        
    }
}