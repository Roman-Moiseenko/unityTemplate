using System;
using Game.GamePlay.Classes;
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

        public Chest OpeningChest()
        {
            return CellOpening.CurrentValue == 0 ? null : Chests[CellOpening.CurrentValue];
        }

        
        
    }
}