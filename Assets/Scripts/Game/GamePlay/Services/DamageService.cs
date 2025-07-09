using System.Collections.Generic;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.States;
using Game.GamePlay.View.Mobs;
using Game.GamePlay.View.Towers;
using Game.State.Root;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.Services
{
    public class DamageService
    {
        private readonly FsmGameplay _fsmGameplay;
        private readonly GameplayStateProxy _gameplayState;
        private readonly WaveService _waveService;
        private readonly TowersService _towersService;
        private readonly RewardProgressService _rewardProgressService;
        private IObservableCollection<TowerViewModel> AllTowers;
        // private 

        public DamageService(
            FsmGameplay fsmGameplay,
            GameplayStateProxy gameplayState,
            WaveService waveService, TowersService towersService
        )
        {
            _fsmGameplay = fsmGameplay;
            _gameplayState = gameplayState;
            _waveService = waveService;
            _towersService = towersService;

            AllTowers = _towersService.AllTowers;

            waveService.AllMobsMap.ObserveAdd().Subscribe(e =>
            {
                var mobEntity = e.Value.Value;
//                Debug.Log(mobEntity.UniqueId + " " + mobEntity.Health.CurrentValue + " " + mobEntity.IsDead.Value);
                mobEntity.IsDead.Skip(1).Subscribe(
                    v =>
                    {
                        if (v)
                        {
                            gameplayState.Progress.Value += 5;
                            waveService.AllMobsMap.Remove(e.Value.Key);
                        }
                    }
                );
            });


            fsmGameplay.Fsm.StateCurrent.Subscribe(e =>
            {
                if (e.GetType() == typeof(FsmStateGamePlay))
                {
                }
            });
        }
    }
}