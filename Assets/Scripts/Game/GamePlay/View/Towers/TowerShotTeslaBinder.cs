using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.GamePlay.View.Mobs;
using Game.State.Maps.Mobs;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Towers
{
    public class TowerShotTeslaBinder : TowerShotBinder
    {
        private List<ShotTeslaBinder> missiles = new();
        public override void FirePrepare(MobViewModel mobViewModel)
        {
            _target = mobViewModel.PositionTarget;
            //Делаем дубль объекта
            var shotTeslaBinder = Instantiate(missile.GetComponent<ShotTeslaBinder>(), transform);
            shotTeslaBinder.Bind(mobViewModel);
            missiles.Add(shotTeslaBinder);
        }

        public override void FireFinish()
        {
            foreach (var teslaBinder in missiles.ToList())
            {
                Destroy(teslaBinder.gameObject);
                missiles.Remove(teslaBinder);
            }
        }

        public override void FireStart()
        {
            //Debug.Log("Tesla FireStart " + missiles.Count);
            //yield return new WaitForSeconds(_viewModel.Speed);
            //yield return new WaitUntil(() => _viewModel.GameSpeed.Value != 0);
        }
        
    }
}