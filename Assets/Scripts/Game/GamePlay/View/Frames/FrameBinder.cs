using R3;
using UnityEngine;

namespace Game.GamePlay.View.Frames
{
    public class FrameBinder : MonoBehaviour
    {
        private FrameViewModel _frameViewModel;
        [SerializeField] private Material allowed;
        [SerializeField] private Material forbidden;
        [SerializeField] private GameObject frame;
        public void Bind(FrameViewModel frameViewModel)
        {
            _frameViewModel = frameViewModel;

            frameViewModel.Enable.Subscribe(newValue =>
            {
                if (newValue)
                {
                    frame.GetComponent<MeshRenderer>().material = allowed;
                    //Debug.Log("Можно строить");
                }
                else
                {
                    frame.GetComponent<MeshRenderer>().material = forbidden;
                    //Debug.Log("Нельзя строить");
                }
            });
            transform.position = new Vector3(frameViewModel.Position.CurrentValue.x, 0, frameViewModel.Position.CurrentValue.y);
            //TODO Смена материала
        }
    }
}