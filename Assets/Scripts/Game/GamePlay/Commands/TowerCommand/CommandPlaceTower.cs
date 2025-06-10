using MVVM.CMD;
using UnityEngine;

namespace Game.GamePlay.Commands.TowerCommand
{
    public class CommandPlaceTower : ICommand
    {
        public readonly string TowerTypeId;
        public readonly Vector2Int Position;

        public CommandPlaceTower(string towerTypeId, Vector2Int position)
        {
            TowerTypeId = towerTypeId;
            Position = position;
        }
    }
}