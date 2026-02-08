using UnityEngine;

namespace Game.Common
{
    public static class MyFunc
    {
        
        public static string CurrencyToStr(long value)
        {
            float t;
            switch (value)
            {
                case >= 1000000000:
                    t = (int)(value / 10000000) / 100f;
                    return $"{t}B";
                case >= 1000000:
                    t = (int)(value / 10000) / 100f;
                    return $"{t}M";
                case >= 1000:
                    t = (int)(value / 10) / 100f;
                    return $"{t}K";
                default:
                    return value.ToString();
            }
        }

        public static Vector3 Vector2To3(Vector2 vector)
        {
            return new Vector3(vector.x, 0, vector.y);
        }
        public static Vector3 Vector2To3(Vector2Int vector)
        {
            return new Vector3(vector.x, 0, vector.y);
        }
        
        public static Vector2 Vector3To2(Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }
        
    }
}