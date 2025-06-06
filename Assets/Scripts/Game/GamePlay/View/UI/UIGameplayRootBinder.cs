using Game.GamePlay.View.UI.PanelActions;
using Game.GamePlay.View.UI.PanelBuild;
using MVVM.UI;
using UnityEngine;

namespace Game.GamePlay.View.UI
{
    public class UIGameplayRootBinder : UIRootBinder
    {
        //Если нужно свое, то делаем override OnBind() 
        protected override void OnBind(UIRootViewModel viewModel)
        {
            

            //viewModel.OpenedPanels
            
            //_windowsContainer.AddPanel(PanelBuild);
          //  Debug.Log(" **** " + viewModel.GetType());
        }
    }
}
