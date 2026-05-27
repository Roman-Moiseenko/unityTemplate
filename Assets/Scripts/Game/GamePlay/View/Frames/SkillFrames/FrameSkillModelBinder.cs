using System;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Frames.SkillFrames
{
    public abstract class FrameSkillModelBinder : MonoBehaviour
    {
        protected FrameSkillViewModel ViewModel;
        protected DisposableBag _disposables;

        public void Bind(FrameSkillViewModel viewModel)
        {
            ViewModel = viewModel;
            OnBind();
        }

        protected abstract void OnBind();


        protected virtual void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}