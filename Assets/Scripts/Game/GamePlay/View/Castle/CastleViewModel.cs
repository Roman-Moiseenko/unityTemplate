using Game.GamePlay.Services;
using Game.GamePlay.View.Mobs;
using Game.State.Maps.Castle;
using Game.State.Maps.Mobs;
using Game.State.Maps.Shots;
using Game.State.Root;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Castle
{
    public class CastleViewModel
    {
        private readonly GameplayStateProxy _gameplayState;
        public CastleEntity CastleEntity { get; }
        public ReadOnlyReactiveProperty<int> Level { get; }
        public readonly string ConfigId;
        public Vector2Int Position { get; }
        public ReactiveProperty<MobViewModel> MobTarget = new();
        public float Speed => CastleEntity.Speed;
        
        public CastleViewModel(CastleEntity castleEntity,
            GameplayStateProxy gameplayState)
        {
            _gameplayState = gameplayState;
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

        public void SetTarget(MobViewModel mobViewModel)
        {
            if (MobTarget.CurrentValue == null) MobTarget.OnNext(mobViewModel);
        }

        public void RemoveTarget(MobViewModel mobViewModel)
        {
            if (MobTarget.CurrentValue == mobViewModel) MobTarget.OnNext(null);
        }

        public void SetDamageAfterShot()
        {
            //Доп.проверка на случай убийства моба
            if (MobTarget.CurrentValue == null) return;
            var shot = new ShotData
            {
                Damage = CastleEntity.Damage,
                DamageType = DamageType.Normal, //TODO Возможно сделать крит-шанс
                Position = MobTarget.CurrentValue.PositionTarget.CurrentValue,
                Single = true,
                MobEntityId = MobTarget.CurrentValue.UniqueId,
            };
            _gameplayState.Shots.Add(shot);
        }
    }
}