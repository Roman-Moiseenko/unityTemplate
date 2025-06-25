using System.Collections.Generic;
using Game.State.Maps.Roads;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.GamePlay.Services
{
    public class WayService
    {

        public Vector2Int GetLastPoint(List<RoadEntityData> roads)
        {
            var result = roads[roads.Count - 1].Position;
            return result;
        }


        public Vector2Int GetExitPoint(List<RoadEntityData> roads)
        {
            var road = roads[^1];
            var point = GetVectorExit(road);
      //      Debug.Log("point = " + point + " road = " + road.Position + ", " + road.Rotate);
            if (roads.Count > 1)
            {
                var preRoad = roads[^2];
                if (point == preRoad.Position) //Точка выхода совпала с предыдущей дорогой
                {
            //        Debug.Log("Точка выхода совпала с предыдущей дорогой = ");
                    point = GetVectorEnter(road);
             //       Debug.Log("point = " + point);
                }
            }
            
            return point;
        }
        
        public Vector2Int GetEnterPoint(List<RoadEntityData> roads)
        {
            var road = roads[0];
            var point = GetVectorEnter(road);
            if (roads.Count > 1)
            {
                var nextRoad = roads[1];
                if (point == nextRoad.Position)
                {
                    point = GetVectorExit(road);
                }
            }
            
            return point;
        }
        
        
   /*     public Vector2Int GetExitPoint(List<RoadEntityData> roads)
        {
            var road = roads[roads.Count - 1];
            
            return GetVectorExit(road);
        }
        */
        
        public Vector2Int GetFirstPoint(List<RoadEntityData> roads)
        {
            return roads[0].Position;
        }



        private Vector2Int GetVectorExit(RoadEntityData road, bool _t = false)
        {
            if (_t)
            {
                Debug.Log("road.Rotate " + road.Rotate);
                Debug.Log("position " + road.Position);
                Debug.Log("IsTurn " + road.IsTurn);
            }
            
            var delta = Vector2Int.zero;
            if (road.IsTurn)
            {
                switch (road.Rotate % 4)
                {
                    case 0: 
                        delta = new Vector2Int(0, 1);    
                        break;
                    case 1: 
                        delta = new Vector2Int(1, 0);
                        break;
                    case 2: 
                        delta = new Vector2Int(0, -1);
                        break;
                    case 3: 
                        delta = new Vector2Int(-1, 0);
                        break;
                }
            }
            else
            {
                switch (road.Rotate % 4)
                {
                    case 0:
                        delta = new Vector2Int(1, 0); //<-
                        break;
                    case 1: 
                        delta = new Vector2Int(0, 1); 
                        break;
                    case 2: 
                        delta = new Vector2Int(-1, 0); // ->
                        break;
                    case 3: 
                        delta = new Vector2Int(0, -1);
                        break;
                }
            }
            
            return road.Position + delta;
        }
        
        private Vector2Int GetVectorEnter(RoadEntityData road)
        {
            var delta = Vector2Int.zero;
            if (road.IsTurn)
            {
                switch (road.Rotate % 4)
                {
                    case 0: 
                        delta = new Vector2Int(-1, 0);    
                        break;
                    case 1: 
                        delta = new Vector2Int(0, 1);
                        break;
                    case 2: 
                        delta = new Vector2Int(1, 0);
                        break;
                    case 3: 
                        delta = new Vector2Int(0, -1);
                        break;
                }
            }
            else
            {
                switch (road.Rotate % 4)
                {
                    case 0:
                        delta = new Vector2Int(-1, 0); //->
                        break;
                    case 1: 
                        delta = new Vector2Int(0, -1); 
                        break;
                    case 2: 
                        delta = new Vector2Int(1, 0); // <-
                        break;
                    case 3: 
                        delta = new Vector2Int(0, 1);
                        break;
                }
            }
            
            return road.Position + delta;
        }
    }
}