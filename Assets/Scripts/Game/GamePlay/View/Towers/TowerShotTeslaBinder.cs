using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.State.Maps.Mobs;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Towers
{
    public class TowerShotTeslaBinder : TowerShotBinder
    {
        private List<ShotTeslaBinder> missiles = new();
        public override void FirePrepare(MobEntity mobEntity)
        {
            _target = mobEntity.PositionTarget;
            //Делаем дубль объекта
            var shotTeslaBinder = Instantiate(missile.GetComponent<ShotTeslaBinder>(), transform);
            shotTeslaBinder.Bind(mobEntity);
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

        public override IEnumerator FireStart()
        {
            //Debug.Log("Tesla FireStart " + missiles.Count);
            yield return new WaitForSeconds(_viewModel.SpeedFire.CurrentValue);
            yield return new WaitUntil(() => _viewModel.GameSpeed.Value != 0);
        }
        
    }
}