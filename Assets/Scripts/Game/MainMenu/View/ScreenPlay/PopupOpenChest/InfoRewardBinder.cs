using System;
using MVVM.UI;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.MainMenu.View.ScreenPlay.PopupOpenChest
{
    public class InfoRewardBinder : DropdownBinder<InfoRewardViewModel>
    {
        protected override void OnBind(InfoRewardViewModel viewModel)
        {
            //TODO Добавляем элементы
        }
        
        public override void OpenDropdown()
        {
            container.gameObject.SetActive(true);
        }

        public override void CloseDropdown()
        {
            container.gameObject.SetActive(false);
        }


    }
}