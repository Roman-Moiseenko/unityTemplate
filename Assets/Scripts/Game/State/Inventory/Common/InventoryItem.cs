using System;
using Cysharp.Threading.Tasks;
using R3;

namespace Game.State.Inventory.Common
{
    public abstract class InventoryItem : IDisposable
    {
        public InventoryItemData Origin { get; }
        public InventoryType TypeItem => Origin.TypeItem;
        public string ConfigId => Origin.ConfigId;
        public int UniqueId => Origin.UniqueId;
        public string Name => Origin.Name;
        public readonly ReactiveProperty<long> Amount;
        protected DisposableBag _disposables;
        
        protected InventoryItem(InventoryItemData data)
        {
            Origin = data;
            Amount = new ReactiveProperty<long>(data.Amount);
            Amount
                .Subscribe(newAmount => data.Amount = newAmount)
                .AddTo(ref _disposables); 
        }

        public T As<T>() where T : InventoryItem
        {
            return (T)this;
        }

        public virtual void Dispose()
        {
            Amount?.Dispose();
            _disposables.Dispose();
        }
    }
}