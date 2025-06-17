using MVVM.CMD;
using UnityEngine;

namespace Game.GamePlay.Commands.TowerCommand
{
    public class CommandMoveTower : ICommand
    {
        public readonly int UniqueId;
        public readonly Vector2Int Position;

        public CommandMoveTower(int uniqueId, Vector2Int position)
        {
            UniqueId = uniqueId;
            Position = position;
        }
    }
}