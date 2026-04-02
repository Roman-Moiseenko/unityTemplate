using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GameRoot.View.Input.TabPanel
{
    public class TabGroup : MonoBehaviour
    {
        public List<TabButton> tabButtons = new();
        public Sprite tabIdle;
        public Sprite tabHover;
        public Sprite tabActive;
        [SerializeField] private TabButton SelectedTab;

        public List<GameObject> tabs = new();
        
        private void OnEnable()
        {
            if (SelectedTab != null)
            {
                SelectedTab.Select();
            }
        }

        public void Subscribe(TabButton tabButton)
        {
            tabButtons.Add(tabButton);
        }

        public void OnTabEnter(TabButton tabButton)
        {
            ResetTabs();
            if (SelectedTab == null || tabButton != SelectedTab)
            {
                tabButton.background.sprite = tabHover;
            }
        }

        public void OnTabExit(TabButton tabButton)
        {
           ResetTabs();
        }

        public void OnTabSelected(TabButton tabButton)
        {
            if (SelectedTab != null)
            {
                SelectedTab.Deselect();
            }
            SelectedTab = tabButton;
            
            ResetTabs();
            
            SelectedTab.background.sprite = tabActive;
            var index = tabButton.transform.GetSiblingIndex();
            for (var i = 0; i < tabs.Count; i++)
            {
                if (i != index) tabs[i].gameObject.SetActive(false);
            }
            tabs[index].gameObject.SetActive(true);
            SelectedTab.Select();
        }

        private void ResetTabs()
        {
            foreach (var tabButton in tabButtons)
            {
                if (SelectedTab != tabButton) tabButton.background.sprite = tabIdle;
            }
        }
    }
}