using UnityEngine;

namespace Game.GamePlay.View.Frames
{
    public class FrameBinder : MonoBehaviour
    {
        public void Bind(Vector3 position)
        {
            transform.position = position;
        }
    }
}