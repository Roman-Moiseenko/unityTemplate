using System.Collections.Generic;
using Game.State.Entities;
using Game.State.Maps.Castle;
using Game.State.Maps.Grounds;
using Game.State.Maps.Roads;
using Game.State.Maps.Towers;
using Game.State.Maps.Waves;

namespace Game.State.Root
{
    public class GameplayState
    {

        public int MapId { get; set; }
        public int CurrentWave { get; set; }
        public int GlobalEntityId { get; set; }

        public int GameSpeed { get; set; } = 1;
        public int PreviousGameSpeed { get; set; }
        
        public int Progress { get; set; } //Текущий прогресс игры, растет от убийства мобов, при = наполнении обнуляем и увеличиваем ProgressLevel на 1
        public int ProgressLevel { get; set; } //Влияет на коэффициент роста Progress в обратно-пропорциональном порядке 

        public int SoftCurrency { get; set; } //При входе 0
        public int HardCurrency { get; set; } //Дублирование?
        
        //Список наград
        //Список Волн
        //Список Мобов
        public CastleEntityData CastleData { get; set; } 
        
        public int Id { get; set; }
        public List<TowerEntityData> Towers;  //Либо разделить на Tower и Ground 

        public List<GroundEntityData> Grounds;
        
        public List<RoadEntityData> Way; //Основная Дорога
        public List<RoadEntityData> WaySecond;
        public List<RoadEntityData> WayDisabled;

        //public List<WaveEntityData> Waves;
        public Dictionary<int, WaveEntityData> Waves;
        
        public int CreateEntityID()
        {
            return GlobalEntityId++;
        }

        
    }
}