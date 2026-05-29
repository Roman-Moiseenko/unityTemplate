using System;
using Game.State.Maps.Skills;
using UnityEngine;
using UnityEngine.VFX;

namespace Game.GamePlay.View.Frames.SkillFrames
{
    public class FrameSkill01ModelBinder : FrameSkillModelBinder
    {
        [SerializeField] private VisualEffect bottomArea;
        protected override void OnBind()
        {
            if (ViewModel.Parameters.TryGetValue(SkillParameterType.Range, out var range))
            {
                var currentRadius = range.Value / 2;
                bottomArea.SetFloat("Radius", currentRadius);
              //  bottomArea.Play();
            }
            else
            {
                throw new Exception("Не найден параметр радиуса");
            }
        }
    }
}