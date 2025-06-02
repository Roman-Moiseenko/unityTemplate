using UnityEngine;

namespace Game.GamePlay.View.Grounds
{
    public class GroundBinder : MonoBehaviour
    {
        public void Bind(GroundViewModel viewModel)
        {
            transform.position = new Vector3(
                viewModel.Position.CurrentValue.x,
                0,
                viewModel.Position.CurrentValue.y
            );
        }
    }
}