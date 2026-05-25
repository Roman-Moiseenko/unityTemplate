using System;
using Cysharp.Threading.Tasks;
using Game.State.Common;
using Game.State.Inventory.Common;
using Game.State.Maps.Mobs;
using Game.State.Maps.Towers;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.State.Inventory.TowerCards
{
    public class TowerCard : InventoryItem
    {
        public ReactiveProperty<TypeEpic> EpicLevel;
        public readonly ReactiveProperty<int> Level;
        public ObservableDictionary<TowerParameterType, TowerParameter> Parameters;
     //   public TypeDefence Defence;
       // public ObservableDictionary<TowerParameterType, TowerParameter> BaseParameters;
        
        public TowerCard(TowerCardData data) : base(data)
        {
         //   Defence = data.Defence;
            EpicLevel = new ReactiveProperty<TypeEpic>(data.EpicLevel);
            EpicLevel.Subscribe(newValue => data.EpicLevel = newValue).AddTo(ref _disposables);
            
            Level = new ReactiveProperty<int>(data.Level);
            Level.Subscribe(newAmount => data.Level = newAmount).AddTo(ref _disposables);
            
            Parameters = new ObservableDictionary<TowerParameterType, TowerParameter>();
            //Debug.Log(JsonConvert.SerializeObject(data, Formatting.Indented));
            foreach (var parameter in data.Parameters)
            {
                Parameters.Add(parameter.Key, new TowerParameter(parameter.Value));
            }

            Parameters.ObserveAdd().Subscribe(e =>
            {
                var key = e.Value.Key;
                var value = e.Value.Value;
                Origin.As<TowerCardData>().Parameters.Add(key, value.Origin);
            }).AddTo(ref _disposables);
            Parameters.ObserveRemove().Subscribe(e =>
            {
                var key = e.Value.Key;
                Origin.As<TowerCardData>().Parameters.Remove(key);
            }).AddTo(ref _disposables);
            Parameters.ObserveClear().Subscribe(_ =>
            {
                Origin.As<TowerCardData>().Parameters.Clear();
            }).AddTo(ref _disposables);
            Parameters.ObserveChanged().Subscribe(newValue =>
            {
                var ket = newValue.NewItem.Key;
                var value = newValue.NewItem.Value;
               // Debug.Log($"{ket} + {value}");
                //TODO Протестить, может и не понадобится
            }).AddTo(ref _disposables);

            //     TowerType = data.TowerType;
        }

        


        public int MaxLevel()
        {
            return Convert.ToInt32(EpicLevel.CurrentValue + 1) * 10;
        }
        
        public int GetCostPlanLevelUpTowerCard()
        {
            var levelCost = (Level.CurrentValue / 5 + 1); 
            return levelCost * 2;
        }
        
        public int GetCostCurrencyLevelUpTowerCard()
        {
            var levelCost = (Level.CurrentValue / 5 + 1); 
            return levelCost * 1000;
        }

        public override void Dispose()
        {
            //TODO Очистить параметры
            EpicLevel?.Dispose();
            foreach (var (key, parameter) in Parameters)
            {
                parameter?.Dispose();
            }
            Parameters.Clear();
            Level?.Dispose();
            base.Dispose();
        }
    }
}