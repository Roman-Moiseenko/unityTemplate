using DI;
using Game.State;
using Game.State.Gameplay;
using Game.State.Gameplay.Statistics;
using Game.State.Inventory;
using Game.State.Maps.Mobs;

namespace Game.GamePlay.View.UI.PopupStatistics
{
    public class StatisticElementViewModel
    {
        public GameplayStateProxy GameplayState { get; set; }
        public MobDefence? Defence = null;

        public int MaxLevel;

        public float Damage;
        public float Percent = 0f;
        public string Name;
        public string ConfigId;
        public int Level = 0;
        public TypeEpicCard EpicCard;
        public int Count;
        public TypeEntityStatisticDamage TypeEntity;
        
        public StatisticElementViewModel(DIContainer container)
        {
            GameplayState = container.Resolve<IGameStateProvider>().GameplayState;

        }

    }
}