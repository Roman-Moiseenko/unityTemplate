using MVVM.CMD;
using UnityEngine;

namespace Game.GamePlay.Commands.TowerCommand
{
    public class CommandPlaceTower : ICommand
    {
        public readonly string ConfigId;
        public readonly Vector2Int Position;
        public Vector2Int Placement { get; set; }

        public CommandPlaceTower(string configId, Vector2Int position)
        {
            ConfigId = configId;
            Position = position;
        }

        
    }
}