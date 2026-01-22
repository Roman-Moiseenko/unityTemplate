using System;
using Game.GamePlay.View.Mobs;
using UnityEngine;

namespace Game.GamePlay.View.Castle
{
    public class CastleVisibleBinder : MonoBehaviour
    {
        private CastleViewModel _viewModel;

        public void Bind(CastleViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!other.gameObject.CompareTag("Mob")) return; //Обрабатываем только мобов
            if (_viewModel.MobTarget.CurrentValue == null) //Если цели нет, назначаем первую вошедшую
            {
                var mobBinder = other.gameObject.GetComponent<MobBinder>();
                _viewModel.SetTarget(mobBinder.ViewModel);
            }
        }

        private void OnCollisionStay(Collision other)
        {
            if (!other.gameObject.CompareTag("Mob")) return; //Обрабатываем только мобов
            if (_viewModel.MobTarget.CurrentValue == null) //Если цели нет, назначаем первую в области поражения
            {
                var mobBinder = other.gameObject.GetComponent<MobBinder>();
                _viewModel.SetTarget(mobBinder.ViewModel);
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (!other.gameObject.CompareTag("Mob")) return; //Обрабатываем только мобов
            if (_viewModel.MobTarget.CurrentValue != null)
            {
                var mobBinder = other.gameObject.GetComponent<MobBinder>();
                _viewModel.RemoveTarget(mobBinder.ViewModel);
            }
        }
    }
}