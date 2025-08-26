using System.Collections.Generic;
using System.Linq;
using Game.State.Inventory;
using Game.State.Maps.Towers;
using UnityEngine;

namespace Game.GameRoot.ImageManager
{
    public class ImageManagerBinder : MonoBehaviour
    {
        [SerializeField] private List<ImageItemByEpicType> epicLevels;
        [SerializeField] private List<ImageItemByConfig> towerPlan;
        [SerializeField] private List<ImageItemByConfigLevel> towerCard;
        [SerializeField] private List<ImageItemByParameter> parameters;



        public Sprite GetEpicLevel(TypeEpicCard typeEpicCard)
        {
            return epicLevels.FirstOrDefault(t => t.TypeEpic == typeEpicCard)!.Sprite;
        }

        public Sprite GetTowerCard(string configId, int level)
        {
            var items = towerCard.FirstOrDefault(t => t.ConfigId == configId)!.ByLevels;
            return items.FirstOrDefault(t => t.Level == level)!.Sprite;
        }

        public Sprite GetParameter(TowerParameterType type)
        {
            return parameters.FirstOrDefault(t => t.TypeParameter == type)!.Sprite;
        }

        public Sprite GetTowerPlan(string configId)
        {
            return towerPlan.FirstOrDefault(t => t.ConfigId == configId)!.Sprite;
        }
    }
}