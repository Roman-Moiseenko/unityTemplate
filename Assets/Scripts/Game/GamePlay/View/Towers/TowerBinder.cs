
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Towers
{
    public class TowerBinder : MonoBehaviour
    {
        private TowerViewModel _viewModel;
        public void Bind(TowerViewModel viewModel)
        {
            _viewModel = viewModel;
            _viewModel.Position.Subscribe(newPosition => transform.position = new Vector3(newPosition.x, 0, newPosition.y));
            
            transform.position = new Vector3(
                viewModel.Position.CurrentValue.x,
                0,
                viewModel.Position.CurrentValue.y
            );
        }
    }
}