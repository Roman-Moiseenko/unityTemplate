using System;
using System.Collections.Generic;
using System.Linq;
using Game.State.Inventory;
using Game.State.Inventory.Chests;
using Game.State.Maps.Mobs;
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
        [SerializeField] private List<ImageItemByDefence> defences;
        [SerializeField] private List<ImageItemByConfig> otherSprite;
        [SerializeField] private List<ImageItemByConfig> roads;
        [SerializeField] private List<ImageItemByChest> chests;
        [SerializeField] private List<ImageItemByConfig> grounds;


        public Sprite GetEpicLevel(TypeEpicCard typeEpicCard)
        {
            return epicLevels.FirstOrDefault(t => t.TypeEpic == typeEpicCard)!.Sprite;
        }
        public Sprite GetChest(TypeChest? typeChest)
        {
            if (typeChest == null) return GetOther("ChestNot");
            
            return chests.FirstOrDefault(t => t.TypeChest == typeChest)!.Sprite;
        }
        
        public Sprite GetEpicLevel(string indexEpic)
        {
            foreach (TypeEpicCard typeEpic in Enum.GetValues(typeof(TypeEpicCard)))
            {
                if (typeEpic.Index() == int.Parse(indexEpic))
                    return epicLevels.FirstOrDefault(t => t.TypeEpic == typeEpic)!.Sprite;
            }
            return null;
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

        public Sprite GetDefence(MobDefence? defence)
        {
            return defence == null
                ? null
                : defences.FirstOrDefault(t => t.Defence == defence)!.Sprite;
        }

        public Sprite GetOther(string configId)
        {
            return otherSprite.FirstOrDefault(t => t.ConfigId == configId)!.Sprite;
        }

        public Sprite GetRoad(string configId)
        {
            return roads.FirstOrDefault(t => t.ConfigId == configId)!.Sprite;
        }
        
        public Texture GetGround(string configId)
        {
            return grounds.FirstOrDefault(t => t.ConfigId == configId)!.Texture;
        }
    }
}