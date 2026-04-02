using System;
using System.Collections.Generic;
using System.Linq;
using Game.State.Common;
using Game.State.Inventory;
using Game.State.Inventory.Chests;
using Game.State.Maps.Mobs;
using Game.State.Maps.Skills;
using Game.State.Maps.Towers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.GameRoot.ImageManager
{
    public class ImageManagerBinder : MonoBehaviour
    {
        [SerializeField] private List<ImageItemByEpicType> epicLevels;
        [SerializeField] private List<ImageItemByConfig> towerPlan;
        [SerializeField] private List<ImageItemByConfigLevel> towerCard;
        [SerializeField] private List<ImageItemByConfig> skillPlan;
        [SerializeField] private List<ImageItemByConfig> skillCard;
        [SerializeField] private List<ImageItemByTowerParameter> towerParameters;
        [SerializeField] private List<ImageItemBySkillParameter> skillParameters;
        //[SerializeField] private List<ImageItemByDefence> defences;
        [SerializeField] private List<ImageItemByConfig> otherSprite;
        [SerializeField] private List<ImageItemByConfig> roads;
        [SerializeField] private List<ImageItemByChest> chests;
        [SerializeField] private List<ImageItemByConfig> grounds;
        [SerializeField] private List<ImageEpicData> epicMaps;
        [SerializeField] private List<ImageDefenceData> defenceMaps;


        public ImageDefenceData GetDefenceData(TypeDefence defence)
        {
            return defenceMaps.FirstOrDefault(t => t.Defence == defence);
        }
        
        public ImageEpicData GetEpicData(TypeEpic typeEpicCard)
        {
            return epicMaps.FirstOrDefault(t => t.Epic == typeEpicCard);
        }
        
        //TODO Удалить 
        public Sprite GetEpicLevel(TypeEpic typeEpicCard)
        {
            return epicLevels.FirstOrDefault(t => t.TypeEpic == typeEpicCard)!.Sprite;
        }
        
        public Sprite GetEpicLevel(string indexEpic)
        {
            foreach (TypeEpic typeEpic in Enum.GetValues(typeof(TypeEpic)))
            {
                if (typeEpic.Index() == int.Parse(indexEpic))
                    return epicLevels.FirstOrDefault(t => t.TypeEpic == typeEpic)!.Sprite;
            }
            return null;
        }        
        
        public Sprite GetChest(TypeChest? typeChest)
        {
            if (typeChest == null) return GetOther("ChestNot");
            
            return chests.FirstOrDefault(t => t.TypeChest == typeChest)!.Sprite;
        }
        
        //public Dictionary<string> 

        public Sprite GetTowerCard(string configId, int level)
        {
            var items = towerCard.FirstOrDefault(t => t.ConfigId == configId)!.ByLevels;
            return items.FirstOrDefault(t => t.Level == level)!.Sprite;
        }

        public Sprite GetTowerParameter(TowerParameterType type)
        {
            return towerParameters.FirstOrDefault(t => t.TypeParameter == type)!.Sprite;
        }
        public Sprite GetSkillParameter(SkillParameterType type)
        {
            return skillParameters.FirstOrDefault(t => t.TypeParameter == type)!.Sprite;
        }
        public Sprite GetTowerPlan(string configId)
        {
            return towerPlan.FirstOrDefault(t => t.ConfigId == configId)!.Sprite;
        }
        public Sprite GetSkillCard(string configId)
        {
            return skillCard.FirstOrDefault(t => t.ConfigId == configId)!.Sprite;
        }
        public Sprite GetSkillPlan(string configId)
        {
            return skillPlan.FirstOrDefault(t => t.ConfigId == configId)!.Sprite;

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