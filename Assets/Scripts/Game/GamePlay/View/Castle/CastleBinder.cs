using UnityEngine;

namespace Game.GamePlay.View.Castle
{
    public class CastleBinder : MonoBehaviour
    {
        public void Bind(CastleViewModel viewModel)
        {
            transform.position = new Vector3(
                viewModel.Position.CurrentValue.x,
                1,
                viewModel.Position.CurrentValue.y
            );
        }
    }
}