using System;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Hero
{
    public class HeroBinder : MonoBehaviour
    {
        private HeroViewModel _viewModel;
        
        private DisposableBag _disposables;

        public void Bind(HeroViewModel viewModel)
        {
            _viewModel = viewModel;
        }


        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}