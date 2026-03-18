using R3;

namespace Game.State.Gameplay.Statistics
{
    public class StatisticGame
    {
        public StatisticGameData Origin;

        public ReactiveProperty<int> CountKills { get; private set; }
        public ReactiveProperty<int> CountTowers { get; private set; }
        public ReactiveProperty<int> CountRoads { get; private set; }
        public ReactiveProperty<float> AllDamage { get; private set; }

        public StatisticGame(StatisticGameData statisticGameData)
        {
            Origin = statisticGameData;
            CountKills = new ReactiveProperty<int>(Origin.CountKills);
            CountKills.Subscribe(v => Origin.CountKills = v);
            
            CountTowers = new ReactiveProperty<int>(Origin.CountTowers);
            CountTowers.Subscribe(v => Origin.CountTowers = v);
            
            CountRoads = new ReactiveProperty<int>(Origin.CountRoads);
            CountRoads.Subscribe(v => Origin.CountRoads = v);
            
            AllDamage = new ReactiveProperty<float>(Origin.AllDamage);
            AllDamage.Subscribe(v => Origin.AllDamage = v);
        }

        public void KillMob()
        {
            CountKills.Value++;
        }

        public void BuildTower()
        {
            CountTowers.Value++;
        }

        public void DestroyTower()
        {
            CountRoads.Value--;
        }

        public void BuildRoad()
        {
            CountRoads.Value++;
        }

        public void SetDamage(float damage, string configId)
        {
            AllDamage.Value += damage;
            var pair = Origin.Damages.Find(p => p.Key == configId);
            if (pair != null)
            {
                pair.Value += damage;
            }
            else
            {
                pair = new PairStringFloat
                {
                    Key = configId,
                    Value = damage
                };
                Origin.Damages.Add(pair);
            }

        }
    }
}
