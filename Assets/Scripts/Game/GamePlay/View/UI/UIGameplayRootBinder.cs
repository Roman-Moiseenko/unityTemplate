using MVVM.UI;
using UnityEngine;

namespace Game.GamePlay.View.UI
{
    public class UIGameplayRootBinder : UIRootBinder
    {
        //Если нужно свое, то делаем override OnBind() 
        protected override void OnBind(UIRootViewModel viewModel)
        {
            Debug.Log(" **** " + viewModel.GetType());
        }
    }
}
