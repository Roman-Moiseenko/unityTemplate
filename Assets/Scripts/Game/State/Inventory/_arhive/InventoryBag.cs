using System;
using System.Linq;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.State.Inventory
{
    public abstract class InventoryBag<T, TF> : IInventoryBag
        where T : InventoryItem
        where TF: InventoryItemData  
    {
        public InventoryBagData<TF> Origin;
        public ObservableList<T> Items = new();
        public abstract bool Accumulation { get; }

        protected InventoryBag(InventoryBagData<TF> bagData)
        {
            Origin = bagData;
            
            bagData.Items.ForEach(item =>
            {
                Items.Add((T)InventoryFactory.Create(item));
            });
            Items.ObserveAdd().Subscribe(e =>
            {
                Origin.Items.Add((TF)e.Value.Origin);
            });
            Items.ObserveRemove().Subscribe(e =>
            {
                Origin.Items.Remove((TF)e.Value.Origin);
            });
        }
        
        public void AddItem(T item)
        {
            if (Accumulation)
            {
                foreach (var inventoryItem in Items)
                {
                    if (inventoryItem.ConfigId != item.ConfigId) continue;
                    inventoryItem.Amount.Value += item.Amount.Value;
                    return;
                }
            }
            else
            {
                if (item.Amount.CurrentValue > 1) throw new Exception("Должно быть 1");
            }

            Items.Add(item);
        }
        

        public void Remove(T item, int amount)
        {
            throw new System.NotImplementedException();
        }

        public T GetItem(int id)
        {
            return Items.FirstOrDefault(item => item.UniqueId == id);
        }

        public void Sort()
        {
            throw new System.NotImplementedException();
        }
        
        public TD As<TD>() where TD : InventoryBag<T, TF>
        {
            return (TD)this;
        }
    }
}