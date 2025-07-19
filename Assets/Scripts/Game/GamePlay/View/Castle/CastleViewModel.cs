﻿using System.Collections.Generic;
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
        
        public Vector2Int Position { get; }

        public CastleViewModel(
            CastleEntity castleEntity,
            CastleService castleService
        )
        {
            ConfigId = castleEntity.ConfigId;
            _castleEntity = castleEntity;
            _castleService = castleService;
            

            Position = castleEntity.Position;
        }

        public bool IsPosition(Vector2 position)
        {
            float delta = 0.5f; //Половина ширины клетки
            int _x0 = Position.x;
            int _y0 = Position.y;
         //   Debug.Log($" *** position.x {position.x} < {_x0 + delta} && > {_x0 - delta}");
         //   Debug.Log($" *** position.y {position.y} < {_y0 + delta} && > {_y0 - delta}");
            if ((position.x < _x0 + delta && position.x > _x0 - delta) && 
                (position.y < _y0 + delta && position.y > _y0 - delta))
                return true;
            return false;
        }
    }
}