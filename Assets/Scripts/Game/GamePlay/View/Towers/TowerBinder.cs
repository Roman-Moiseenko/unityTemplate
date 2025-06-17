
using UnityEngine;

namespace Game.GamePlay.View.Towers
{
    public class TowerBinder : MonoBehaviour
    {
        public void Bind(TowerViewModel viewModel)
        {
            transform.position = new Vector3(
                viewModel.Position.CurrentValue.x,
                0,
                viewModel.Position.CurrentValue.y
            );
        }
    }
}