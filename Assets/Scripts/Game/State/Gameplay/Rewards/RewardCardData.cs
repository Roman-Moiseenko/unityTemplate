using Game.State.Common;
using Game.State.Inventory;
using UnityEngine;

namespace Game.State.Gameplay.Rewards
{
    public class RewardCardData
    {
        public RewardType RewardType;
        public string TargetId; //Цель Конфиг башни
        public int Level; //Уровень наград
        public string ConfigId; //Конфиг бустера ()
        public int Direction; //Направление 1-4
        public Vector2Int Position;
        public int UniqueId;
        public object UniqueId2;
      //  public string Caption;
      //  public string Description;
        public string Name;
        public bool OnRoad;
        public TypeEpic EpicLevel;
     //   public MobDefence Defence;


        public bool IsBuild()
        {
            if (RewardType == RewardType.Ground || RewardType == RewardType.Tower ||
                RewardType == RewardType.Road)
                return true;
            return false;
        }
    }
}