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
        }
    }
}