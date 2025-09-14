using Game.Common;
using Game.GameRoot.ImageManager;
using Game.State.Inventory.Chests;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainMenu.View.ScreenPlay.PopupFinishGameplay.PrefabBinders
{
    public class ChestRewardBinder : MonoBehaviour
    {
        [SerializeField] private Image imageChest;
        [SerializeField] private TMP_Text textChest;

        public void Bind(TypeChest? typeChest)
        {
            var imageManager = GameObject.Find(AppConstants.IMAGE_MANAGER).GetComponent<ImageManagerBinder>();
            imageChest.sprite = imageManager.GetChest(typeChest);
            
            textChest.text = typeChest.GetString();
        }
    }
}