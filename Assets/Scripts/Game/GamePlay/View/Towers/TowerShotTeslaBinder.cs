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
/*
        public override void Bind(TowerViewModel viewModel)
        {
            IsFree = true;
            Debug.Log("TowerShotTeslaBinder");
            _viewModel = viewModel;
            shotBinder.gameObject.SetActive(false);
            var d = Disposable.CreateBuilder();
            _disposable = d.Build();

        }
        */
   /*     public override void FirePrepare(MobViewModel mobViewModel)
        {
            _targetPosition = mobViewModel.PositionTarget;
            //Делаем дубль объекта
            Debug.Log("FirePrepare " + mobViewModel.UniqueId);
            var shotTeslaBinder = Instantiate(missile.GetComponent<ShotTeslaBinder>(), transform);
            shotTeslaBinder.Bind(mobViewModel);
            missiles.Add(shotTeslaBinder);
            //TODO Анимация разрадов
        }
        */
/*
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
        public override void StopShot()
        {
            FireFinish();
        }
        */
    }
}