using Game.Common;
using Game.GameRoot.ImageManager;
using Game.State.Maps.Towers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PanelBuild
{
    public class CardParameterBinder : MonoBehaviour
    {
        [SerializeField] private TMP_Text textName;
        [SerializeField] private TMP_Text textValue;
        [SerializeField] private TMP_Text textUpgrade;
        [SerializeField] private Image imageParameter;
        private ImageManagerBinder _imageManager;

        private void Awake()
        {
            _imageManager = GameObject.Find(AppConstants.IMAGE_MANAGER).GetComponent<ImageManagerBinder>();
        }

        public void Bind(TowerParameterType parameterType, Vector2 value)
        {
            imageParameter.sprite = _imageManager.GetParameter(parameterType);
            textName.text = parameterType.GetString();
            textValue.text = value.x.ToString() + " " + parameterType.GetMeasure();
            if (value.y == 0)
            {
                textUpgrade.gameObject.SetActive(false);
            }
            else
            {
                textUpgrade.text = value.y.ToString();
                textUpgrade.gameObject.SetActive(true);
            }
            gameObject.SetActive(true);
        }
    }
}