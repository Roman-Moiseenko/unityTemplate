using Game.Common;
using Game.GameRoot.ImageManager;
using Game.State.Common;
using Game.State.Maps.Mobs;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GameRoot.View.Defence
{
    public class DefenceBinder : MonoBehaviour
    {
        [SerializeField] private Image backDefence;
        [SerializeField] private Image iconDefence;
        private ImageManagerBinder _imageManager;
        private void Awake()
        {
            _imageManager = GameObject.Find(AppConstants.IMAGE_MANAGER).GetComponent<ImageManagerBinder>();
        }
        
        public void Bind(TypeDefence? defence)
        {
            if (defence == null)
            {
                gameObject.SetActive(false);
            }
            else
            {
                var defenceImage = _imageManager.GetDefenceData((TypeDefence)defence);
                backDefence.sprite = defenceImage.Background;
                iconDefence.sprite = defenceImage.Icon;
                gameObject.SetActive(true);
            }
        }
    }
}