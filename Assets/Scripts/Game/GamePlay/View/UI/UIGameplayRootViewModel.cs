using DI;
using Game.GamePlay.Services;
using MVVM.UI;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.UI
{
    public class UIGameplayRootViewModel : UIRootViewModel
    {
        //Делаем свои кастомные фичи для сцены

        public UIGameplayRootViewModel(DIContainer container) : base(container)
        {

        }
    }
}