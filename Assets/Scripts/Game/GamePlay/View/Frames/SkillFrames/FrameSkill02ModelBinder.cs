using R3;
using UnityEngine;

namespace Game.GamePlay.View.Frames.SkillFrames
{
    public class FrameSkill02ModelBinder : FrameSkillModelBinder
    {
        [SerializeField] private Transform model;
        protected override void OnBind()
        {
            var material = model.GetComponent<Renderer>().material;
            material.SetInt("_Selected", 1);
            
            ViewModel.IsPlacement.Subscribe(v =>
            {
                material.SetInt("_Enabled", v ? 1 : 0);
            }).AddTo(ref _disposables);
            
            ViewModel.Direction.Subscribe(p =>
            {
                var v3 = new Vector3(p.x, 0, p.y);
                if (v3 != Vector3.zero) model.rotation = Quaternion.LookRotation(v3);
            }).AddTo(ref _disposables);

            //Поворот model в сторону дороги
            //Размещение только на дороге - model смена цвета
        }
    }
}  