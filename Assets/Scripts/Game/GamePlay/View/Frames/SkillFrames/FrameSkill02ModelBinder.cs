using UnityEngine;

namespace Game.GamePlay.View.Frames.SkillFrames
{
    public class FrameSkill02ModelBinder : FrameSkillModelBinder
    {
        [SerializeField] private Transform model;
        protected override void OnBind()
        {
            //Поворот model в сторону дороги
            //Размещение только на дороге - model смена цвета
        }
    }
}  