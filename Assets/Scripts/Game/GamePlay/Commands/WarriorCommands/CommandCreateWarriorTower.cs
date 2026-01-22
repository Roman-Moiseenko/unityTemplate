using Game.State.Maps.Towers;
using MVVM.CMD;
using UnityEngine;

namespace Game.GamePlay.Commands.WarriorCommands
{
    public class CommandCreateWarriorTower : ICommand
    {
        public string ConfigId;
        public int UniqueId;
        public Vector2Int Position;
        public TowerTypeEnemy TypeEnemy;
        public Vector2Int Placement { get; set; }
        public CommandCreateWarriorTower()
        {
            
        }

        
    }
}