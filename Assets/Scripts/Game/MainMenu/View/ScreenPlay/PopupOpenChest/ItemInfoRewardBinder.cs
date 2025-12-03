using Game.GameRoot.View.ResourceReward;
using TMPro;
using UnityEngine;

namespace Game.MainMenu.View.ScreenPlay.PopupOpenChest
{
    public class ItemInfoRewardBinder : MonoBehaviour
    {
        [SerializeField] private ResourceRewardBinder rewardBinder;
        [SerializeField] private Transform background;
        [SerializeField] private TMP_Text title;

        public void Bind(ItemInfoRewardViewModel viewModel, int numberPosition)
        {
            //Debug.Log(numberPosition);
            background.gameObject.SetActive(viewModel.Odd);
            title.text = viewModel.Title;
            rewardBinder.Bind(viewModel.ResourceRewardViewModel);
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                gameObject.GetComponent<RectTransform>().anchoredPosition.x, 
                -150 - numberPosition * 70);
            //TODO Меняем позицию Y для gameObject
        }
    }
}