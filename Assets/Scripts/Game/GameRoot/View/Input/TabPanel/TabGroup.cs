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
        
        private void Start()
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
            SelectedTab.Select();
            ResetTabs();
            SelectedTab.background.sprite = tabActive;
         //   tabButton.SizeUp();
            var index = tabButton.transform.GetSiblingIndex();
            for (int i = 0; i < tabs.Count; i++)
            {
                tabs[i].gameObject.SetActive(i == index);
            }
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