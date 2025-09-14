using System;
using DI;
using Game.State.Inventory.Chests;
using R3;

namespace Game.MainMenu.View.ScreenPlay.Chests
{
    public class CellChestViewModel
    {
        public ReactiveProperty<Chest> ChestEntity;

        public ReactiveProperty<bool> IsOpening;
        private readonly PlayUIManager _uiManager;

        public CellChestViewModel(ContainerChests containerChests, DIContainer container)
        {
            IsOpening = new ReactiveProperty<bool>();
            containerChests.CellOpening.Subscribe(v => IsOpening.OnNext(v == 0));
            ChestEntity = new ReactiveProperty<Chest>(null);
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
            
            if (ChestEntity.Value.Status.CurrentValue == StatusChest.Close)
            {
                _uiManager.OpenPopupOpenChest();
            }
        }
    }
}