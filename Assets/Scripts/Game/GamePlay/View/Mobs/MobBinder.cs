using UnityEngine;

namespace Game.GamePlay.View.Mobs
{
    public class MobBinder : MonoBehaviour
    {
		public MobViewModel _viewModel;
        public void Bind(MobViewModel viewModel)
        {
            _viewModel = viewModel;
        }
        
    }
}