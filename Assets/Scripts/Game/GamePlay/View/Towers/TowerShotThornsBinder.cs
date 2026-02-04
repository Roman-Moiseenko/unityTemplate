using System.Collections;
using Game.GamePlay.View.Mobs;

namespace Game.GamePlay.View.Towers
{
    public class TowerShotThornsBinder : TowerShotBinder
    {
        private MobViewModel _mobViewModel;

        public override void Bind(TowerAttackViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public override void FireToTarget(MobViewModel mobViewModel)
        {
            _mobViewModel = mobViewModel;
            if (mainCoroutine != null) StopCoroutine(mainCoroutine);

            IsFree = false;
            mainCoroutine = StartCoroutine(StartShotFire());
        }
        
        protected override IEnumerator StartShotFire()
        {
            yield return null;
            _viewModel.SetDamageAfterShot(_mobViewModel);
            IsFree = true;
            yield return null;
        }
        public override void StopShot()
        {
        }
    }
}