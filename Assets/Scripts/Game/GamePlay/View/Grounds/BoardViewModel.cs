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
            
            //Debug.Log($"countAngle = {countAngle} countOutAngle = {countOutAngle}  countSide = {countSide} Position = {boardEntity.Position.CurrentValue}"); //Euler

            //Внешние углы
            if (boardEntity.Origin.BottomOutAngle)
            {
                Walls.Add(new BoardWallViewModel
                    {
                        ConfigId = "OutAngle",
                        Id = $"OutAngleBottom-{ConfigId}",
                        Rotation = 180,
                        DeltaX = 1,
                        DeltaY = 1,
                    }
                );
            }
            if (boardEntity.Origin.TopOutAngle)
            {
                Walls.Add(new BoardWallViewModel
                    {
                        ConfigId = "OutAngle",
                        Id = $"OutAngleTop-{ConfigId}",
                        Rotation = 0,
                        DeltaX = -1,
                        DeltaY = -1,
                    }
                );
            }
            if (boardEntity.Origin.RightOutAngle)
            {
                Walls.Add(new BoardWallViewModel
                    {
                        ConfigId = "OutAngle",
                        Id = $"OutAngleRight-{ConfigId}",
                        Rotation = 90,
                        DeltaX = -1,
                        DeltaY = 1,
                    }
                );
            }
            if (boardEntity.Origin.LeftOutAngle)
            {
                Walls.Add(new BoardWallViewModel
                    {
                        ConfigId = "OutAngle",
                        Id = $"OutAngleLeft-{ConfigId}",
                        Rotation = -90,
                        DeltaX = 1,
                        DeltaY = -1,
                    }
                );
            }
            
            //Внутренние углы
            if (boardEntity.Origin.BottomInAngle)
            {
                Walls.Add(new BoardWallViewModel
                    {
                        ConfigId = "InAngle",
                        Id = $"InAngleBottom-{ConfigId}",
                        Rotation = 180,
                        DeltaX = 1,
                        DeltaY = 1,
                    }
                );
            }
            if (boardEntity.Origin.TopInAngle)
            {
                Walls.Add(new BoardWallViewModel
                    {
                        ConfigId = "InAngle",
                        Id = $"InAngleTop-{ConfigId}",
                        Rotation = 0,
                        DeltaX = -1,
                        DeltaY = -1,
                    }
                );
            }
            if (boardEntity.Origin.RightInAngle)
            {
                Walls.Add(new BoardWallViewModel
                    {
                        ConfigId = "InAngle",
                        Id = $"InAngleRight-{ConfigId}",
                        Rotation = 90,
                        DeltaX = -1,
                        DeltaY = 1,
                    }
                );
            }
            if (boardEntity.Origin.LeftInAngle)
            {
                Walls.Add(new BoardWallViewModel
                    {
                        ConfigId = "InAngle",
                        Id = $"InAngleLeft-{ConfigId}",
                        Rotation = -90,
                        DeltaX = 1,
                        DeltaY = -1,
                    }
                );
            }
            
            //Стороны, добавить отключение краев стен
            if (boardEntity.Origin.BottomSide)
            {
                Walls.Add(new BoardWallViewModel
                    {
                        ConfigId = "Side",
                        Id = $"SideBottom-{ConfigId}",
                        Rotation = 180,
                        DeltaY = 1,
                        ShowRightSide = !boardEntity.Origin.RightInAngle,
                        ShowLeftSide = !boardEntity.Origin.BottomInAngle,
                    }
                );
            }
            if (boardEntity.Origin.TopSide)
            {
                Walls.Add(new BoardWallViewModel
                    {
                        ConfigId = "Side",
                        Id = $"SideTop-{ConfigId}",
                        Rotation = 0,
                        DeltaY = -1,
                        ShowRightSide = !boardEntity.Origin.LeftInAngle,
                        ShowLeftSide = !boardEntity.Origin.TopInAngle,
                    }
                );
            }
            if (boardEntity.Origin.RightSide)
            {
                Walls.Add(new BoardWallViewModel
                    {
                        ConfigId = "Side",
                        Id = $"SideRight-{ConfigId}",
                        Rotation = 90,
                        DeltaX = -1,
                        ShowRightSide = !boardEntity.Origin.TopInAngle,
                        ShowLeftSide = !boardEntity.Origin.RightInAngle,
                    }
                );
            }
            if (boardEntity.Origin.LeftSide)
            {
                Walls.Add(new BoardWallViewModel
                    {
                        ConfigId = "Side",
                        Id = $"SideLeft-{ConfigId}",
                        Rotation = -90,
                        DeltaX = 1,
                        ShowRightSide = !boardEntity.Origin.BottomInAngle,
                        ShowLeftSide = !boardEntity.Origin.LeftInAngle,
                    }
                );
            }
        }
    }
}