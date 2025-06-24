using UnityEngine;

namespace Game.GamePlay.Classes
{
    public struct RectBorder
    {
        public float BottomX;
        public float BottomY;
        public float TopX;
        public float TopY;

        public RectBorder(int centreX, int centreY, int width, int height)
        {
            BottomX = centreX - width / 2;
            BottomY = centreY - height / 2;
            TopX = centreX + width / 2;
            TopY = centreY + height / 2;
        }
        
        public RectBorder(int centreX, int centreY, int width, int height, Vector2Int offset)
        {
            BottomX = centreX - width / 2 - offset.x;
            BottomY = centreY - height / 2 - offset.y;
            TopX = centreX + width / 2 + offset.x;
            TopY = centreY + height / 2 + offset.y;
        }
    }
}