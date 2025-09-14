using System;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.State.Inventory.Chests
{
    public class ContainerChests
    {
        public ContainerChestsData Origin;
        public ObservableDictionary<int, Chest> Chests;
        public const int MaxChest = 4;

        public ReactiveProperty<long> StartOpening;
        public ReactiveProperty<int> CellOpening;

        public ContainerChests(ContainerChestsData chestsData)
        {
            Origin = chestsData;
            StartOpening = new ReactiveProperty<long>(chestsData.StartOpening);
            StartOpening.Subscribe(newValue => chestsData.StartOpening = newValue);
            //Debug.Log("chestsData.CellOpening = " + chestsData.CellOpening);
            CellOpening = new ReactiveProperty<int>(chestsData.CellOpening);
            CellOpening.Subscribe(newValue => chestsData.CellOpening = newValue);
            
            Chests = new ObservableDictionary<int, Chest>();
            foreach (var (cell, chestData) in chestsData.Chests)
            {
                Chests.Add(cell, new Chest(chestData));
            }

            Chests.ObserveAdd().Subscribe(e =>
            {
                var cell = e.Value.Key;
                var chest = e.Value.Value;
                chestsData.Chests.Add(cell, chest.Origin);
            });
            Chests
                .ObserveRemove()
                .Subscribe(e => { chestsData.Chests.Remove(e.Value.Key); });
            
        }
        public int AddChest(int levelChest, int wave)
        {
            for (var i = 1; i <= MaxChest; i++)
            {
                if (Chests.TryGetValue(i, out var value)) continue;
                
                var epic = wave switch 
                {
                    100 => TypeChest.Diamond,
                    > 60 => TypeChest.Ruby,
                    > 30 => TypeChest.Gold,
                    _ => TypeChest.Silver
                };
                    
                var chest = new ChestEntityData
                {
                    Level = levelChest,
                    TimeStart = 0,
                    TypeChest = epic
                };
                Chests.Add(i, new Chest(chest));
                return i;
            }

            return 0;
        }

        public void StartOpeningChest(int cell)
        {
            CellOpening.OnNext(cell);
            StartOpening.OnNext(DateTime.Now.ToUniversalTime().ToFileTimeUtc());
            Chests[cell].Status.OnNext(StatusChest.Opening);
        }

        public bool IsOpening()
        {
            return CellOpening.CurrentValue != 0;
        }

        public void OpenedChest(int cell)
        {
            CellOpening.OnNext(0);
            StartOpening.OnNext(0);
            Chests[cell].Status.OnNext(StatusChest.Opened);
        }

        public void ForcedOpening(int cell, int minutes)
        {
            
        }
    }
}