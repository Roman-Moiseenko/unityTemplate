using System.Collections.Generic;
using DI;
using Game.GamePlay.Services;
using Game.MainMenu.Services;
using Game.State.Inventory;
using Game.State.Inventory.TowerCards;
using Game.State.Maps.Towers;
using UnityEngine;

namespace Game.MainMenu.View.ScreenInventory.PopupBlacksmith.PrefabBinders
{
    public class InfoUpgradedViewModel
    {

        public string NameTower;
        public string NameEpic;
        public Dictionary<string, Vector2> Parameters = new();
        

        public InfoUpgradedViewModel()
        {
            
            
        }
        
    }
}