using DI;
using MVVM.UI;
using UnityEngine;

namespace Game.GamePlay.View.UI.PopupB
{
    public class PopupBViewModal : WindowViewModel
    {
        public PopupBViewModal(DIContainer container) : base(container)
        {
        }

        public override string Id => "PopupB";
        public override string Path => "Gameplay/Popups/";
        
        //Выйти из игры
    }
}