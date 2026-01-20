using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.State.Maps.Mobs;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Towers
{
    public class TowerShotBarrackBinder : TowerShotBinder
    {

        public override void FirePrepare(MobEntity mobEntity)
        {
            _target = mobEntity.PositionTarget;
        }

        public override void FireFinish()
        {

        }

        public override IEnumerator FireStart()
        {
            yield return new WaitForSeconds(_viewModel.SpeedFire);
        }
        
    }
}