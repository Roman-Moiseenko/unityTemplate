using Game.Common;
using Game.GameRoot.ImageManager;
using Game.State.Maps.Skills;
using Game.State.Maps.Towers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PanelBuild.CardBackend
{
    public class BackendParameterBinder : MonoBehaviour
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
            imageParameter.sprite = _imageManager.GetTowerParameter(parameterType);
            textName.text = parameterType.GetString();
            var isDamage = parameterType.IsDamage();
            var measure = parameterType.GetMeasure();

            _Bind(value, isDamage, measure);
            
         /*   var valOrigin = value.x;
            var valUpgrade = parameterType.IsDamage()
                ? value.x * value.y / 100
                : value.y;
            
            valOrigin = (float)System.Math.Round(valOrigin, (valOrigin < 100) ? 2 : 1);
            valUpgrade = (float)System.Math.Round(valUpgrade, 1);


            textValue.text = $"{valOrigin}{parameterType.GetMeasure()}";
            if (valUpgrade == 0)
            {
                textUpgrade.gameObject.SetActive(false);
            }
            else
            {
                textUpgrade.text = (valUpgrade > 0 ? "+" : "") +
                                   valUpgrade +
                                   (parameterType.IsDamage() ? "" : "%");
                textUpgrade.gameObject.SetActive(true);
            }

            gameObject.SetActive(true); */
        }
        
        public void Bind(SkillParameterType parameterType, Vector2 value)
        {
            imageParameter.sprite = _imageManager.GetSkillParameter(parameterType);
            textName.text = parameterType.GetString();
            var isDamage = parameterType.IsDamage();
            var measure = parameterType.GetMeasure();

            _Bind(value, isDamage, measure);
            /*
            var valOrigin = value.x;
            var valUpgrade = isDamage
                ? value.x * value.y / 100
                : value.y;
            
            valOrigin = (float)System.Math.Round(valOrigin, (valOrigin < 100) ? 2 : 1);
            valUpgrade = (float)System.Math.Round(valUpgrade, 1);


            textValue.text = $"{valOrigin}{measure}";
            if (valUpgrade == 0)
            {
                textUpgrade.gameObject.SetActive(false);
            }
            else
            {
                textUpgrade.text = (valUpgrade > 0 ? "+" : "") +
                                   valUpgrade +
                                   (isDamage ? "" : "%");
                textUpgrade.gameObject.SetActive(true);
            }

            gameObject.SetActive(true); */
        }

        private void _Bind(Vector2 value, bool isDamage, string measure)
        {
            var valOrigin = value.x;
            var valUpgrade = isDamage
                ? value.x * value.y / 100
                : value.y;
            
            valOrigin = (float)System.Math.Round(valOrigin, (valOrigin < 100) ? 2 : 1);
            valUpgrade = (float)System.Math.Round(valUpgrade, 1);


            textValue.text = $"{valOrigin}{measure}";
            if (valUpgrade == 0)
            {
                textUpgrade.gameObject.SetActive(false);
            }
            else
            {
                textUpgrade.text = (valUpgrade > 0 ? "+" : "") +
                                   valUpgrade +
                                   (isDamage ? "" : "%");
                textUpgrade.gameObject.SetActive(true);
            }

            gameObject.SetActive(true);
        }
        
    }
}