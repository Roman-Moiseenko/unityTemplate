using System.Collections.Generic;
using Game.State.Entities;

namespace Game.State.Root
{
    public class GameplayState
    {

        public int MapId { get; set; }
        public int CurrentWave { get; set; }
        public int GlobalEntityId { get; set; }
        
        public int GameSpeed { get; set; }
        public int PreviousGameSpeed { get; set; }
        
        public int Progress { get; set; } //Текущий прогресс игры, растет от убийства мобов, при = наполнении обнуляем и увеличиваем ProgressLevel на 1
        public int ProgressLevel { get; set; } //Влияет на коэффициент роста Progress в обратно-пропорциональном порядке 

        public int SoftCurrency { get; set; } //При входе 0
        public int HardCurrency { get; set; } //Дублирование?
        
        //Список наград
        //Список Волн
        //Список Мобов
        
        public int Id { get; set; }
        public List<EntityData> Entities;
        //Либо разделить на Tower Ground Road Build
        
        public int CreateEntityID()
        {
            return GlobalEntityId++;
        }

        
    }
}