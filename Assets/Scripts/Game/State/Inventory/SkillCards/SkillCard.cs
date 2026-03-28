using System;
using Game.State.Common;
using Game.State.Inventory.Common;
using Game.State.Maps.Skills;
using ObservableCollections;
using R3;

namespace Game.State.Inventory.SkillCards
{
    public class SkillCard : InventoryItem
    {
        public ReactiveProperty<TypeEpic> EpicLevel;
        public readonly ReactiveProperty<int> Level;
        public ObservableDictionary<SkillParameterType, SkillParameter> Parameters;

        public SkillCard(SkillCardData data) : base(data)
        {
            EpicLevel = new ReactiveProperty<TypeEpic>(data.EpicLevel);
            EpicLevel.Subscribe(newValue => data.EpicLevel = newValue);


            Level = new ReactiveProperty<int>(data.Level);
            Level.Subscribe(newAmount => data.Level = newAmount);

            Parameters = new ObservableDictionary<SkillParameterType, SkillParameter>();
            //Debug.Log(JsonConvert.SerializeObject(data, Formatting.Indented));
            foreach (var parameter in data.Parameters)
            {
                Parameters.Add(parameter.Key, new SkillParameter(parameter.Value));
            }

            Parameters.ObserveAdd().Subscribe(e =>
            {
                var key = e.Value.Key;
                var value = e.Value.Value;
                Origin.As<SkillCardData>().Parameters.Add(key, value.Origin);
            });
            Parameters.ObserveRemove().Subscribe(e =>
            {
                var key = e.Value.Key;
                Origin.As<SkillCardData>().Parameters.Remove(key);
            });
            Parameters.ObserveClear().Subscribe(_ => { Origin.As<SkillCardData>().Parameters.Clear(); });
            Parameters.ObserveChanged().Subscribe(newValue =>
            {
                var ket = newValue.NewItem.Key;
                var value = newValue.NewItem.Value;
                // Debug.Log($"{ket} + {value}");
                //TODO Протестить, может и не понадобится
            });
        }

        public int MaxLevel()
        {
            return Convert.ToInt32(EpicLevel.CurrentValue + 1) * 10;
        }
        
        public int GetCostPlanLevelUpSkillCard()
        {
            var levelCost = (Level.CurrentValue / 5 + 1); 
            return levelCost * 2;
        }
        
        public int GetCostCurrencyLevelUpSkillCard()
        {
            var levelCost = (Level.CurrentValue / 5 + 1); 
            return levelCost * 1000;
        } 
    }
}