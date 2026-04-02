using System;
using System.Collections.Generic;
using Game.MainMenu.View.ScreenInventory.Deck;
using Game.MainMenu.View.ScreenInventory.Panels;
using Game.MainMenu.View.ScreenInventory.SkillCards;
using Game.MainMenu.View.ScreenInventory.SkillPlans;
using Game.MainMenu.View.ScreenInventory.TowerCards;
using Game.MainMenu.View.ScreenInventory.TowerPlans;
using MVVM.UI;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.MainMenu.View.ScreenInventory
{
    public class ScreenInventoryBinder : WindowBinder<ScreenInventoryViewModel>
    {

        [SerializeField] private DeckBinder deckBinder;
        [SerializeField] private PanelTowersBinder panelTowersBinder;
        [SerializeField] private PanelSkillsBinder panelSkillsBinder;
        [SerializeField] private PanelDetailsBinder panelDetailsBinder;
        [SerializeField] private PanelItemsBinder panelItemsBinder;

        private IDisposable _disposable;
        

        protected override void OnBind(ScreenInventoryViewModel viewModel)
        {
            //TODO Сделать пул объектов инвентаря, не загружая. Также как Damage в ScreenGameplay
            
            
            base.OnBind(viewModel);
            
            deckBinder.Bind(viewModel);
            panelTowersBinder.Bind(viewModel);
            panelSkillsBinder.Bind(viewModel);
            panelDetailsBinder.Bind(viewModel);
            panelItemsBinder.Bind(viewModel);
            
            var d = Disposable.CreateBuilder();
            _disposable = d.Build();
        }
        

        private void OnDestroy()
        {
            _disposable.Dispose();
        }
    }
}