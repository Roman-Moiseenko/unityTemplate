using DI;
using MVVM.UI;

namespace Game.GamePlay.View.UI.PopupStatistics
{
    public class PopupStatisticsViewModel : WindowViewModel
    {
        public override string Id => "PopupStatistics";
        public override string Path => "Gameplay/Popups/";        
        
        public PopupStatisticsViewModel(DIContainer container) : base(container)
        {
            
        }


    }
}