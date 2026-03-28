using Game.Common;
using Game.GameRoot.ImageManager;
using Game.State.Common;
using Game.State.Gameplay.Statistics;
using Game.State.Maps.Mobs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PopupStatistics
{
    public class StatisticElementBinder : MonoBehaviour
    {
        [SerializeField] private Image backgroundCard;
        [SerializeField] private Image iconCard;
        [SerializeField] private Image backgroundDefence;
        [SerializeField] private Image iconDefence;
        [SerializeField] private TMP_Text countEntity;
        
        [SerializeField] private TMP_Text txtName;
        [SerializeField] private TMP_Text txtPercent;
        [SerializeField] private TMP_Text txtDamage;
        [SerializeField] private TMP_Text txtNotDamage;
        [SerializeField] private Slider sliderPercent;
        [SerializeField] private StatisticsStarsBinder starsBinder;
        
        private readonly Vector3 _scaleSmall = new (1, 1, 1);
        private readonly Vector3 _scaleBig = new(1.5f, 1.5f, 1.5f);

        private ImageManagerBinder _imageManager;

        private void Awake()
        {
            _imageManager = GameObject.Find(AppConstants.IMAGE_MANAGER).GetComponent<ImageManagerBinder>();
        }
        public void Bind(StatisticElementViewModel viewModel)
        {
            if (viewModel.TypeEntity == TypeEntityStatisticDamage.Tower)
            {
                backgroundCard.sprite = _imageManager.GetEpicLevel(viewModel.EpicCard);
                iconCard.sprite = _imageManager.GetTowerCard(viewModel.ConfigId, 1);
                iconCard.GetComponent<RectTransform>().localScale = _scaleSmall;
            }
            
            if (viewModel.TypeEntity == TypeEntityStatisticDamage.Skill)
            {
                backgroundCard.sprite = _imageManager.GetEpicLevel(viewModel.EpicCard);
                //iconCard.sprite = _imageManager.GetSkillCard(viewModel.ConfigId, 1);
                iconCard.GetComponent<RectTransform>().localScale = _scaleSmall;
            }
            
            if (viewModel.TypeEntity == TypeEntityStatisticDamage.Hero)
            {
                //backgroundCard.sprite = _imageManager.GetOther(viewModel.ConfigId);
                //iconCard.sprite = _imageManager.GetHeroCard(viewModel.ConfigId, 1);
                iconCard.GetComponent<RectTransform>().localScale = _scaleBig;
            }
            
            if (viewModel.TypeEntity == TypeEntityStatisticDamage.Castle)
            {
                backgroundCard.sprite = _imageManager.GetOther("CastleBack"); //???
                iconCard.sprite = _imageManager.GetOther("Castle");
                iconCard.GetComponent<RectTransform>().localScale = _scaleBig;
            }
            
            if (viewModel.Defence == null)
            {
                backgroundDefence.gameObject.SetActive(false);
                iconDefence.gameObject.SetActive(false);
            }
            else
            {
                var defenceImage = _imageManager.GetDefenceData((TypeDefence)viewModel.Defence);
                backgroundDefence.sprite = defenceImage.Background;
                iconDefence.sprite = defenceImage.Icon;
                backgroundDefence.gameObject.SetActive(true);
                iconDefence.gameObject.SetActive(true);
            }

            if (viewModel.Count == 0)
            {
                countEntity.gameObject.SetActive(false);
            }
            else
            {
                countEntity.text = $"x{viewModel.Count}";
                countEntity.gameObject.SetActive(true);
            }

            txtName.text = viewModel.Name;
            txtPercent.text = $"{Mathf.RoundToInt(viewModel.Percent)}%";
            txtDamage.text = MyFunc.CurrencyToStr((long)viewModel.Damage);
            txtNotDamage.gameObject.SetActive(viewModel.Damage == 0f);
            txtPercent.gameObject.SetActive(viewModel.Damage != 0f);

            sliderPercent.value = viewModel.Percent / 100f;
            starsBinder.Bind(viewModel.Level, viewModel.MaxLevel);
        }
    }
}