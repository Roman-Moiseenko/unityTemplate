using System;
using System.Linq;
using Game.State.Common;
using Game.State.Inventory.Common;
using Game.State.Maps.Skills;
using Game.State.Parameters;
using ObservableCollections;
using R3;

namespace Game.State.Inventory.SkillCards
{
    public class SkillCard : InventoryItem
    {
        public ReactiveProperty<TypeEpic> EpicLevel;
        public readonly ReactiveProperty<int> Level;
        public ObservableDictionary<ParameterType, Parameter> Parameters;

        public SkillCard(SkillCardData data) : base(data)
        {
            EpicLevel = new ReactiveProperty<TypeEpic>(data.EpicLevel);
            EpicLevel
                .Subscribe(newValue => data.EpicLevel = newValue)
                .AddTo(ref _disposables);


            Level = new ReactiveProperty<int>(data.Level);
            Level
                .Subscribe(newAmount => data.Level = newAmount)
                .AddTo(ref _disposables);

            Parameters = new ObservableDictionary<ParameterType, Parameter>();
            //Debug.Log(JsonConvert.SerializeObject(data, Formatting.Indented));
            foreach (var parameter in data.Parameters)
            {
                Parameters.Add(parameter.Key, new Parameter(parameter.Value));
            }

            Parameters.ObserveAdd().Subscribe(e =>
            {
                var key = e.Value.Key;
                var value = e.Value.Value;
                Origin.As<SkillCardData>().Parameters.Add(key, value.Origin);
            }).AddTo(ref _disposables);
            Parameters.ObserveRemove().Subscribe(e =>
            {
                var key = e.Value.Key;
                var param = e.Value.Value;
                param?.Dispose();
                Origin.As<SkillCardData>().Parameters.Remove(key);
            }).AddTo(ref _disposables);
            Parameters
                .ObserveClear()
                .Subscribe(_ =>
                {
                    foreach (var (key, param) in Parameters.ToList())
                    {
                        param?.Dispose();
                    }
                    Origin.As<SkillCardData>().Parameters.Clear();
                })
                .AddTo(ref _disposables);
            Parameters.ObserveChanged().Subscribe(newValue =>
            {
                var ket = newValue.NewItem.Key;
                var value = newValue.NewItem.Value;
                // Debug.Log($"{ket} + {value}");
                //TODO Протестить, может и не понадобится
            }).AddTo(ref _disposables);
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

        public override void Dispose()
        {
            Parameters.Clear(); //Dispose вызывается в подписке
            EpicLevel?.Dispose();
            Level?.Dispose();
            base.Dispose();
        }
    }
}