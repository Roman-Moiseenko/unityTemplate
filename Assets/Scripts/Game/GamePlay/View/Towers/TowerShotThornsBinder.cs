using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.State.Maps.Mobs;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Towers
{
    public class TowerShotThornsBinder : TowerShotBinder
    {
        public override void FirePrepare(MobEntity mobEntity)
        {

        }

        public override void FireFinish()
        {

        }

        public override IEnumerator FireStart()
        {
            yield return new WaitForSeconds(_viewModel.SpeedFire.CurrentValue);
            yield return new WaitUntil(() => _viewModel.GameSpeed.Value != 0);
        }
        
    }
}