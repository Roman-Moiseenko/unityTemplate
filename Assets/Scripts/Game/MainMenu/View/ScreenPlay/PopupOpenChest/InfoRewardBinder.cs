using System.Collections.Generic;
using MVVM.UI;
using UnityEngine;

namespace Game.MainMenu.View.ScreenPlay.PopupOpenChest
{
    public class InfoRewardBinder : DropdownBinder<InfoRewardViewModel>
    {
        private readonly List<ItemInfoRewardBinder> _createdItemsMap = new();
        protected override void OnBind(InfoRewardViewModel viewModel)
        {
            
            viewModel.ItemsInfoRewardViewModel.ForEach(CreateItemInfoCard);
            container.GetComponent<RectTransform>().sizeDelta = new Vector2(
                container.GetComponent<RectTransform>().rect.width,
                _createdItemsMap.Count * 70 + 100
                );
        }

        private void CreateItemInfoCard(ItemInfoRewardViewModel viewModel)
        {
            const string prefabReward = "Prefabs/UI/MainMenu/ScreenPlay/Popups/ItemInfoReward";
            var rewardPrefab = Resources.Load<ItemInfoRewardBinder>(prefabReward);
            var createdReward = Instantiate(rewardPrefab, container.GetComponent<Transform>());
            createdReward.Bind(viewModel, _createdItemsMap.Count);
            _createdItemsMap.Add(createdReward);
        }

        
    }
}