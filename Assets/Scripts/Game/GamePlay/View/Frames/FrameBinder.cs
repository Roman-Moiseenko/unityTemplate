using R3;
using UnityEngine;

namespace Game.GamePlay.View.Frames
{
    public class FrameBinder : MonoBehaviour
    {
        public void Bind(FrameViewModel frameViewModel)
        {
            transform.position = new Vector3(frameViewModel.Position.CurrentValue.x, 0, frameViewModel.Position.CurrentValue.y);
            //TODO Смена материала
        }
    }
}