using System.Collections.Generic;
using Game.GamePlay.Classes;
using Game.State.Maps.Castle;
using Game.State.Maps.Grounds;
using Game.State.Maps.Rewards;
using Game.State.Maps.Roads;
using Game.State.Maps.Towers;
using Game.State.Maps.Warriors;
using UnityEngine;

namespace Game.State.Root
{
    public class GameplayState
    {
        public int MapId { get; set; }
        public int CurrentWave { get; set; }
        public int GlobalEntityId { get; set; }
        public float GameSpeed { get; set; } = 1f;
        //public int PreviousGameSpeed { get; set; }
        public int Progress { get; set; } //Текущий прогресс игры, растет от убийства мобов, при = наполнении обнуляем и увеличиваем ProgressLevel на 1
        public int ProgressLevel { get; set; } //Влияет на коэффициент роста Progress в обратно-пропорциональном порядке 
        public long SoftCurrency { get; set; } //При входе 0
        //public int HardCurrency { get; set; } //Дублирование?
        public int UpdateCards { get; set; }
        public int KillMobs { get; set; }
        public TypeGameplay TypeGameplay { get; set; }
        
        //Список наград
        //Список Мобов
        public CastleEntityData CastleData { get; set; } 
        
        public int Id { get; set; }
   //     public Vector2 GateWave { get; set; }
     //   public Vector2 GateWaveSecond { get; set; }
        public bool HasWaySecond { get; set; }

        public List<RewardEntityData> RewardEntities = new(); 
        public List<TowerEntityData> Towers;  //Либо разделить на Tower и Ground 

        public List<GroundEntityData> Grounds;
        public List<WarriorEntityData> Warriors = new();
        public List<RoadEntityData> Way; //Основная Дорога
        public List<RoadEntityData> WaySecond;
        public List<RoadEntityData> WayDisabled;
        
        
        public int CreateEntityID()
        {
            return GlobalEntityId++;
        }

        
    }
}