using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Settings.Gameplay.Entities;
using Game.State.Common;
using Game.State.Inventory.Common;
using Game.State.Parameters;
using ObservableCollections;
using R3;

namespace Game.State.Inventory.HeroCards
{
    public class HeroCard : InventoryItem
    {
        public TypeEpic EpicLevel => ((HeroCardData)Origin).EpicLevel;
        public bool Available => ((HeroCardData)Origin).Available;
        public TypeDefence Defence => ((HeroCardData)Origin).Defence;
        
        
        public readonly ReactiveProperty<int> Level;
        public readonly ReactiveProperty<int> Rank;
        
        public ObservableDictionary<ParameterType, Parameter> Parameters;
        public HeroCard(HeroCardData data) : base(data)
        {
            Level = new ReactiveProperty<int>(data.Level);
            Level
                .Subscribe(newAmount => data.Level = newAmount)
                .AddTo(ref _disposables);
            
            Rank = new ReactiveProperty<int>(data.Rank);
            Rank
                .Subscribe(newAmount => data.Rank = newAmount)
                .AddTo(ref _disposables);
            
            Parameters = new ObservableDictionary<ParameterType, Parameter>();
            
            foreach (var parameter in data.Parameters)
            {
                Parameters.Add(parameter.Key, new Parameter(parameter.Value));
            }
            
            Parameters.ObserveAdd().Subscribe(e =>
            {
                var key = e.Value.Key;
                var value = e.Value.Value;
                Origin.As<HeroCardData>().Parameters.Add(key, value.Origin);
            }).AddTo(ref _disposables);
            Parameters.ObserveRemove().Subscribe(e =>
            {
                var key = e.Value.Key;
                var param = e.Value.Value;
                param?.Dispose();
                Origin.As<HeroCardData>().Parameters.Remove(key);
            }).AddTo(ref _disposables);
            Parameters.ObserveClear().Subscribe(_ =>
            {
                foreach (var (key, param) in Parameters.ToList())
                {
                    param?.Dispose();
                }
                Origin.As<HeroCardData>().Parameters.Clear();
            }).AddTo(ref _disposables);
            Parameters.ObserveChanged().Subscribe(newValue =>
            {
                var ket = newValue.NewItem.Key;
                var value = newValue.NewItem.Value;
                //TODO Протестить, может и не понадобится
            }).AddTo(ref _disposables);
        }
        
        public override void Dispose()
        {
            Parameters.Clear(); //Dispose вызывается в подписке
            
            Level?.Dispose();
            Rank?.Dispose();
            base.Dispose();
        }

        public void AddParameter(ParameterSettings parameterSettings)
        {
            Parameters.Add(
                parameterSettings.ParameterType,
                new Parameter(new ParameterData(parameterSettings))
            );
        }
        
        public int MaxLevel()
        {
            return Convert.ToInt32(Rank.CurrentValue) * 15;
        }
        
        public int GetCostFragmentRankUpHeroCard()
        {
            return Rank.CurrentValue switch
            {
                1 => 10,
                2 => 50,
                3 => 100,
                4 => 250,
                5 => 500,
                6 => 1000,
                _ => 0
            };
        }
        
        public int GetCostCurrencyLevelUpHeroCard()
        {
            var levelCost = (Level.CurrentValue * Rank.CurrentValue); 
            return levelCost * 1000;
        }
        
        public int GetCostEssenceLevelUpHeroCard()
        {
            var levelCost = (Level.CurrentValue * Rank.CurrentValue); 
            return levelCost * 20;
        }
        
    }
}