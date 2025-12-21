using Game.GamePlay.Services;
using ObservableCollections;
using UnityEngine;

namespace Game.GamePlay.View.Map
{
    public class MapFogViewModel
    {
        public readonly ObservableList<Vector2Int> AllFogs;
        public MapFogViewModel(GroundsService groundsService)
        {
            AllFogs = groundsService.MapFogs;
        }
    }
}