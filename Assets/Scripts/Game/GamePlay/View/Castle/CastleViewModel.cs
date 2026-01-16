using Game.GamePlay.Services;
using Game.State.Maps.Castle;
using Game.State.Maps.Mobs;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Castle
{
    public class CastleViewModel
    {
        public CastleEntity CastleEntity { get; }
        public ObservableList<MobEntity> Target => CastleEntity.Target;
        public ReadOnlyReactiveProperty<int> Level { get; }
        public readonly string ConfigId;
        public Vector2Int Position { get; }
        public CastleViewModel(CastleEntity castleEntity)
        
        {
            ConfigId = castleEntity.ConfigId;
            CastleEntity = castleEntity;
            Position = castleEntity.Position;
        }

        public bool IsPosition(Vector2 position)
        {
            var delta = 0.5f; //Половина ширины клетки
            var _x0 = Position.x;
            var _y0 = Position.y;
            if ((position.x < _x0 + delta && position.x > _x0 - delta) && 
                (position.y < _y0 + delta && position.y > _y0 - delta))
                return true;
            return false;
        }
        
    }
}