using Game.GamePlay.View.Frames;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Skills
{
    public abstract class SkillBinder : MonoBehaviour
    {

        protected DisposableBag _disposables;
        protected SkillViewModel ViewModel;

        

        public void Bind(SkillViewModel viewModel)
        {
            ViewModel = viewModel;
            ViewModel.ToDestroy.Value = false;

            transform.position = new Vector3(ViewModel.EffectPosition.Value.x, 0, ViewModel.EffectPosition.Value.y);
            OnBind();
        }

        protected abstract void OnBind();

        protected virtual void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}