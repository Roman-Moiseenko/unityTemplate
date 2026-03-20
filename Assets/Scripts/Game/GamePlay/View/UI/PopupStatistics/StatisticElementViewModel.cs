using DI;
using Game.State;
using Game.State.Gameplay;
using Game.State.Inventory;

namespace Game.GamePlay.View.UI.PopupStatistics
{
    public class StatisticElementViewModel
    {
        public GameplayStateProxy GameplayState { get; set; }

        public float Damage;
        public float Percent;
        public string Name;
        public string ConfigId;
        public int Levels;
        public TypeEpicCard EpicCard;
        public int Count;
        
        
        public StatisticElementViewModel(DIContainer container)
        {
            GameplayState = container.Resolve<IGameStateProvider>().GameplayState;

        }

    }
}