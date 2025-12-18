using System.Collections.Generic;
using Game.State.Maps.Grounds;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Grounds
{
    public class BoardViewModel
    {
        public readonly BoardEntity BoardEntity;
        public ReadOnlyReactiveProperty<Vector2Int> Position { get; set; }
        public readonly string ConfigId;
        public readonly int BoardEntityId;

        public List<BoardWallViewModel> Walls = new();
        public BoardViewModel(BoardEntity boardEntity)
        {
            BoardEntity = boardEntity;
            ConfigId = boardEntity.ConfigId;
            Position = boardEntity.Position;
            BoardEntityId = boardEntity.UniqueId;
            
            //TODO Создаем список сторон Walls от углов и сторон
            
            var wall = new BoardWallViewModel();
            Walls.Add(wall);
        }
    }
}