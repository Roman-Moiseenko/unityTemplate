using DI;
using MVVM.UI;
using UnityEngine;

namespace Game.GamePlay.View.UI
{
    public class UIGameplayRootViewModel : UIRootViewModel
    {
        //Делаем свои кастомные фичи для сцены

        public UIGameplayRootViewModel(DIContainer container) : base(container)
        {
            //Подписка на состояния
        }
        
    }
}