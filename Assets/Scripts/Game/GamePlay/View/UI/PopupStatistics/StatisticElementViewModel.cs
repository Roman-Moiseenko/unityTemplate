using DI;
using Game.State;
using Game.State.Common;
using Game.State.Gameplay;
using Game.State.Gameplay.Statistics;
using Game.State.Inventory;
using Game.State.Maps.Mobs;

namespace Game.GamePlay.View.UI.PopupStatistics
{
    public class StatisticElementViewModel
    {
        public GameplayStateProxy GameplayState { get; set; }
        public TypeDefence? Defence = null;

        public int MaxLevel;

        public float Damage;
        public float Percent = 0f;
        public string Name;
        public string ConfigId;
        public int Level = 0;
        public TypeEpic EpicCard;
        public int Count;
        public TypeEntityStatisticDamage TypeEntity;
        
        public StatisticElementViewModel(DIContainer container)
        {
            GameplayState = container.Resolve<IGameStateProvider>().GameplayState;

        }

    }
}