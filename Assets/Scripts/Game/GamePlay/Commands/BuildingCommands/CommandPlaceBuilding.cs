using MVVM.CMD;
using UnityEngine;

namespace Game.GamePlay.Commands
{
    public class CommandPlaceBuilding : ICommand
    {
        public readonly string BuildingTypeId;
        public readonly Vector3Int Position;

        public CommandPlaceBuilding(string buildingTypeId, Vector3Int position)
        {
            BuildingTypeId = buildingTypeId;
            Position = position;
        }
    }
}