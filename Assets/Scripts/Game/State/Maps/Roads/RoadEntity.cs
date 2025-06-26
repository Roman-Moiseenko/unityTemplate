using System.Collections.Generic;
using Game.State.Entities;
using R3;
using UnityEngine;

namespace Game.State.Maps.Roads
{
    public class RoadEntity
    {
        public RoadEntityData Origin { get; }
        public int UniqueId => Origin.UniqueId;
        public string ConfigId => Origin.ConfigId;
        public bool IsTurn => Origin.IsTurn;
        public readonly ReactiveProperty<int> Rotate;

        public readonly ReactiveProperty<Vector2Int> Position;
        
  //      public readonly ReactiveProperty<Vector2Int> PointEnter;
 //       public readonly ReactiveProperty<Vector2Int> PointExit;
        
        public RoadEntity(RoadEntityData roadData)
        {
            Origin = roadData;

            Rotate = new ReactiveProperty<int>(roadData.Rotate);
            Rotate.Subscribe(newRotate =>
            {
                roadData.Rotate = newRotate % 4;
            });
            
            Position = new ReactiveProperty<Vector2Int>(roadData.Position);
            Position.Subscribe(newPosition => roadData.Position = newPosition);
            
      //      PointEnter = new ReactiveProperty<Vector2Int>(roadData.PointEnter);
      //      PointEnter.Subscribe(newValue => roadData.PointEnter = newValue);
            
      //      PointExit = new ReactiveProperty<Vector2Int>(roadData.PointExit);
       //     PointExit.Subscribe(newValue => roadData.PointExit = newValue);
            
            //Обсчет координат ???
            if (Origin.IsTurn)
            {
                switch (Origin.Rotate)
                {
                    case 0: 
                        break;
                    case 1: 
                        break;
                    case 2: 
                        break;
                    case 3: 
                        break;
                }
            }
            else
            {
                switch (Origin.Rotate)
                {
                    case 0: 
                        break;
                    case 1: 
                        break;
                    case 2: 
                        break;
                    case 3: 
                        break;
                }
            }
            
        }

        public bool PositionNear(Vector2Int position)
        {
            var x = Position.CurrentValue.x;
            var y = Position.CurrentValue.y;
        //    Debug.Log("Точка проверки " + position);
        //    Debug.Log("Дорога " + Position.CurrentValue);
            foreach (var nearPosition in GetNearPositions())
            {
             //   Debug.Log("nearPosition " + nearPosition);
                if (position == nearPosition) return true;
            }

            return false;
            
            /*
            if (IsTurn) //Поворот
            {
                switch (Rotate.CurrentValue)
                {
                    case 0:
                        if (position.x == x && position.y == y - 1) return true;
                        if (position.x == x + 1 && position.y == y - 1) return true;
                        if (position.x == x + 1 && position.y == y) return true;
                        break;
                    case 1: 
                        if (position.x == x && position.y == y - 1) return true;
                        if (position.x == x - 1 && position.y == y - 1) return true;
                        if (position.x == x - 1 && position.y == y) return true;
                        break;
                    case 2: 
                        if (position.x == x - 1 && position.y == y) return true;
                        if (position.x == x - 1 && position.y == y + 1) return true;
                        if (position.x == x && position.y == y + 1) return true;
                        break;
                    case 3: 
                        if (position.x == x + 1 && position.y == y) return true;
                        if (position.x == x + 1 && position.y == y + 1) return true;
                        if (position.x == x && position.y == y + 1) return true;
                        break;
                }
            }
            else
            {
                switch (Rotate.CurrentValue)
                {
                    case 0: 
                    case 2: 
                        if (position.x == x && position.y == y - 1) return true;
                        if (position.x == x && position.y == y + 1) return true;
                        break;
                    case 1: 
                    case 3: 
                        if (position.x == x - 1 && position.y == y) return true;
                        if (position.x == x + 1 && position.y == y) return true;    
                        break;
                }
            }
            return false;
            */
        }


        public List<Vector2Int> GetNearPositions()
        {
            var x = Position.CurrentValue.x;
            var y = Position.CurrentValue.y;
            List<Vector2Int> list = new();
            if (IsTurn) //Поворот
            {
                switch (Rotate.CurrentValue % 4)
                {
                    case 0:
                        list.Add(new Vector2Int(x, y - 1));
                        list.Add(new Vector2Int(x + 1, y - 1));
                        list.Add(new Vector2Int(x + 1, y));
                        break;
                    case 1: 
                        list.Add(new Vector2Int(x, y - 1));
                        list.Add(new Vector2Int(x - 1, y - 1));
                        list.Add(new Vector2Int(x - 1, y));
                        break;
                    case 2: 
                        list.Add(new Vector2Int(x - 1, y));
                        list.Add(new Vector2Int(x - 1, y + 1));
                        list.Add(new Vector2Int(x, y + 1));
                        break;
                    case 3: 
                        list.Add(new Vector2Int(x + 1, y));
                        list.Add(new Vector2Int(x + 1, y + 1));
                        list.Add(new Vector2Int(x, y + 1));
                        break;
                }
            }
            else
            {
                switch (Rotate.CurrentValue % 4)
                {
                    case 0: 
                    case 2: 
                        list.Add(new Vector2Int(x, y - 1));
                        list.Add(new Vector2Int(x, y + 1));
                        break;
                    case 1: 
                    case 3:
                        list.Add(new Vector2Int(x - 1, y));
                        list.Add(new Vector2Int(x + 1, y));
                        break;
                }
            }

            return list;
        }
    }
}