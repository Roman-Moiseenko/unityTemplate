using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.GamePlay.View.Mobs;
using Game.State.Maps.Mobs;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Towers
{
    public class TowerShotThornsBinder : TowerShotBinder
    {
        public override void FirePrepare(MobViewModel mobViewModel)
        {

        }

        public override void FireFinish()
        {

        }

        public override void FireStart()
        {
            //yield return new WaitForSeconds(_viewModel.Speed);
          //  yield return new WaitUntil(() => _viewModel.GameSpeed.Value != 0);
        }
        
    }
}