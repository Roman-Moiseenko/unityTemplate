using System;
using Game.State.Maps.Towers;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.State.Inventory.TowerCards
{
    public class TowerCard : InventoryItem
    {
        public ReactiveProperty<TypeEpicCard> EpicLevel;
        public readonly ReactiveProperty<int> Level;
        
        public ObservableDictionary<TowerParameterType, TowerParameter> Parameters;

        public ObservableDictionary<TowerParameterType, TowerParameter> BaseParameters;
        
        public TowerCard(TowerCardData data) : base(data)
        {
            
            EpicLevel = new ReactiveProperty<TypeEpicCard>(data.EpicLevel);
            EpicLevel.Subscribe(newValue => data.EpicLevel = newValue);
            
            Level = new ReactiveProperty<int>(data.Level);
            Level.Subscribe(newAmount => data.Level = newAmount);
            
            Parameters = new ObservableDictionary<TowerParameterType, TowerParameter>();
            //Debug.Log(JsonConvert.SerializeObject(data, Formatting.Indented));
            foreach (var parameter in data.Parameters)
            {
                Parameters.Add(parameter.Key, new TowerParameter(parameter.Value));
            }

            Parameters.ObserveChanged().Subscribe(newValue =>
            {
                var ket = newValue.NewItem.Key;
                var value = newValue.NewItem.Value;
                //TODO Протестить, может и не понадобится
            });

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
    }
}