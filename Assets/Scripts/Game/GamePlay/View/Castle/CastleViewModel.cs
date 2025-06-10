using System.Collections.Generic;
using Game.GamePlay.Services;
using Game.Settings.Gameplay.Entities.Buildings;
using Game.State.Maps.Castle;
using Game.State.Mergeable.Buildings;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Castle
{
    public class CastleViewModel
    {
        private readonly CastleEntity _castleEntity;
        private readonly CastleService _castleService;
        

        public readonly int CastleEntityId;
        public ReadOnlyReactiveProperty<int> Level { get; }
        public readonly string ConfigId;
        
        public ReadOnlyReactiveProperty<Vector2Int> Position { get; }

        public CastleViewModel(
            CastleEntity castleEntity,
            CastleService castleService
        )
        {
            CastleEntityId = castleEntity.UniqueId;
            ConfigId = castleEntity.ConfigId;
            Level = castleEntity.Level;
            _castleEntity = castleEntity;
            _castleService = castleService;
            

            Position = castleEntity.Position;
        }
    }
}