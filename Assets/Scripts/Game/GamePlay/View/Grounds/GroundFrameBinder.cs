using R3;
using UnityEngine;

namespace Game.GamePlay.View.Grounds
{
    public class GroundFrameBinder : MonoBehaviour
    {
        [SerializeField] private Material allowed;
        [SerializeField] private Material forbidden;
        private GroundFrameViewModel _viewModel;
        [SerializeField] private GameObject frame;
        public void Bind(GroundFrameViewModel viewModel)
        {
            _viewModel = viewModel;

            transform.localPosition = new Vector3(
                viewModel.GetPosition().x,
                0,
                viewModel.GetPosition().y);

            viewModel.Enabled.Subscribe(newValue =>
            {
                if (newValue)
                {
                    frame.GetComponent<MeshRenderer>().material = allowed;
                } 
                else
                {
                    frame.GetComponent<MeshRenderer>().material = forbidden;
                }
            });
        }
        
        
    }
}