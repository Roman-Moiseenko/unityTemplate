using UnityEngine;

namespace Game.GamePlay.View.Frames.SkillFrames
{
    public abstract class FrameSkillModelBinder : MonoBehaviour
    {
        protected FrameSkillViewModel ViewModel;

        public void Bind(FrameSkillViewModel viewModel)
        {
            ViewModel = viewModel;
            OnBind();
        }

        protected abstract void OnBind();
    }
}