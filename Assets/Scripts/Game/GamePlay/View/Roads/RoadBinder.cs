using Newtonsoft.Json;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Roads
{
    public class RoadBinder : MonoBehaviour
    {
        public void Bind(RoadViewModel viewModel)
        {
            transform.position = new Vector3(
                viewModel.Position.CurrentValue.x,
                0,
                viewModel.Position.CurrentValue.y
            );

            viewModel.Rotate.Subscribe(newValue =>
            {
                transform.localEulerAngles = new Vector3(0, 90f * newValue,0);
            });
        }
        
    }
    
    
}