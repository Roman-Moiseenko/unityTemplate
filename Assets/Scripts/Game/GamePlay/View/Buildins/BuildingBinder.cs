using UnityEngine;

namespace Game.GamePlay.View.Buildins
{
    public class BuildingBinder : MonoBehaviour
    {
        
        public void Bind(BuildingViewModel viewModel)
        {
            transform.position = viewModel.Position.CurrentValue;
        }
    }
}