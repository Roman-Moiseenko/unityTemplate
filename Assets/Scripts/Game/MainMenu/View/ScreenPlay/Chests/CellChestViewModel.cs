using System;
using DI;
using Game.MainMenu.Services;
using Game.State.Inventory.Chests;
using R3;
using UnityEngine;

namespace Game.MainMenu.View.ScreenPlay.Chests
{
    public class CellChestViewModel
    {
        public ReactiveProperty<Chest> ChestEntity;

        public ReactiveProperty<bool> IsOpening;
        private readonly PlayUIManager _uiManager;
        public ChestService ChestService { get; set; }

        public ReactiveProperty<int> TimeLeft = new(0);
        public ReactiveProperty<int> CostLeft = new(0);
        
        public CellChestViewModel(ContainerChests containerChests, DIContainer container, Chest chestEntity)
        {
            IsOpening = new ReactiveProperty<bool>();
            ChestService = container.Resolve<ChestService>();
            containerChests.CellOpening.Subscribe(v => IsOpening.OnNext(v == 0));
            ChestEntity = new ReactiveProperty<Chest>(chestEntity);
            ChestService.TimeLeft.Subscribe(t =>
            {
                //Debug.Log("1");
                if (ChestEntity.Value == null) return;
               // Debug.Log("2");
                if (ChestService.CellOpening.Value != ChestEntity.Value.Cell) return;
                Debug.Log("3 = " + t);
                TimeLeft.Value = t;
            });
            ChestService.CostLeft.Subscribe(t =>
            {
                if (ChestEntity.Value == null) return;
                if (ChestService.CellOpening.Value != ChestEntity.Value.Cell) return;
                CostLeft.Value = t;
            });
            
            //TODO 
            _uiManager = container.Resolve<PlayUIManager>();
        }

        

        public void SetChest(Chest chestEntity)
        {
            ChestEntity.OnNext(chestEntity);
        }

        public void ClearCell()
        {
            ChestEntity.OnNext(null);
        }

        public void RequestClickCellChest()
        {
            
            if (ChestEntity.Value.Status.CurrentValue is StatusChest.Close or StatusChest.Opening)
            {
                _uiManager.OpenPopupOpenChest(ChestEntity.Value);
            }

            if (ChestEntity.Value.Status.CurrentValue == StatusChest.Opened)
            {
                //var rewards = ChestService.GenerateRewards(ChestEntity.Value);
                var type = ChestEntity.Value.TypeChest;
                var rewards = ChestService.OpenCompletedChest(ChestEntity.Value);
                if (rewards == null)
                {
                    throw new Exception("Ошибка награды");
                }
                _uiManager.OpenPopupRewardChest(type, rewards);
            }
        }
    }
}






