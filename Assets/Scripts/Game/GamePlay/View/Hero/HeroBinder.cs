using System;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Hero
{
    public class HeroBinder : MonoBehaviour
    {
        [SerializeField] private Transform heroContainer;
        private HeroViewModel _viewModel;
        
        private DisposableBag _disposables;

        public void Bind(HeroViewModel viewModel)
        {
            _viewModel = viewModel;

            transform.position = new Vector3(
                viewModel.Position.CurrentValue.x, 0, viewModel.Position.CurrentValue.y);
        }


        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}