using Game.GamePlay.Services;
using Game.Settings.Gameplay.Grounds;
using Game.State.Maps.Grounds;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Grounds
{
    public class GroundViewModel
    {
        private readonly GroundEntity _groundEntity;
        private readonly GroundSettings _groundSettings;
        private readonly GroundsService _groundsService;

        public ReadOnlyReactiveProperty<Vector2Int> Position { get; }
        public readonly string ConfigId;
        public readonly int GroundEntityId;
        public ReadOnlyReactiveProperty<bool> Enabled { get; }

        public GroundViewModel(
            GroundEntity groundEntity,
        //    GroundSettings groundSettings,
            GroundsService groundsService)
        {
            _groundEntity = groundEntity;
    //        _groundSettings = groundSettings;
            _groundsService = groundsService;


            GroundEntityId = groundEntity.UniqueId;
            ConfigId = groundEntity.ConfigId;
            Enabled = groundEntity.Enabled;
            Position = groundEntity.Position;
        }
        
    }
}