
using System;
using System.Collections.Generic;
//using Game.State.Entities.Buildings;
using Game.State.GameResources;
using Game.State.Inventory;
using Game.State.Maps;

namespace Game.State.Root
{
   // [Serializable]
    public class GameState
    {
        public bool HasSessionGame { get; set; }
        public int GlobalEntityId { get; set; }
        public int CurrentMapId { get; set; }
        public int GameSpeed { get; set; } //При выходе из Gameplay сохранять
        
        public List<MapData> Maps { get; set; } //Заменить на выйгранные с полученной наградой
        public List<InventoryData> Inventory { get; set; }
        public List<ResourceData> Resources { get; set; }

        public bool ResumeGame { get; set; } //Привыходе false, при входе true 
        //public GameplayState GameplayState = new(); //Cостояния gameplay игры -- удалить
        
        public int CreateEntityID()
        {
            return GlobalEntityId++;
        }

    }    
}
