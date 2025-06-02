using UnityEngine;

namespace Game.GamePlay.View.Buildings
{
    public class BuildingBinder : MonoBehaviour
    {
        public void Bind(BuildingViewModel viewModel)
        {
            transform.position = new Vector3(
                viewModel.Position.CurrentValue.x,
                1,
                viewModel.Position.CurrentValue.y
            );
        }
    }
}