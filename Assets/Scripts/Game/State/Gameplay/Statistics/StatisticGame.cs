using System;
using System.Collections.Generic;
using R3;

namespace Game.State.Gameplay.Statistics
{
    public class StatisticGame
    {
        public StatisticGameData Origin;

        public List<ConfigEntityDamage> Damages => Origin.Damages;
        public List<ConfigEntityCount> Entities => Origin.Entities;
 
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

        public void BuildTower(string configId)
        {
            CountTowers.Value++;
            var pair = Origin.Entities.Find(p => p.ConfigId == configId);
            if (pair != null)
            {
                pair.Count++;
            }
            else
            {
                pair = new ConfigEntityCount
                {
                    ConfigId = configId,
                    Count = 1
                };
                Origin.Entities.Add(pair);
            }
            
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
            var pair = Origin.Damages.Find(p => p.ConfigId == configId);
            if (pair == null) throw new Exception("Ошибка в статистике");
            pair.Damage += damage;

        }
        
        public int GetTowerCount(string configId)
        {
            var pair = Origin.Entities.Find(p => p.ConfigId == configId);
            return pair?.Count ?? 0;
        }

        /**
         * Инициализация статистики, добавляем все сущности (герои, башни, навыки и замок)
         */
        public void Add(string configId, TypeEntityStatisticDamage typeEntity)
        {
            var isPair = Origin.Damages.Find(p => p.ConfigId == configId);
            if (isPair != null) return;
            
            var pair = new ConfigEntityDamage
            {
                ConfigId = configId,
                Damage = 0,
                TypeEntity = typeEntity
            };
            Origin.Damages.Add(pair);
        }

        public List<ConfigEntityDamage> GetDamages()
        {
            List<ConfigEntityDamage> damages = new();

            var hero = GetDamageHero();
            if (hero != null) damages.Add(hero);
            damages.Add(GetDamageCastle());
            var towers = GetDamageTowers();
            foreach (var tower in towers)
                damages.Add(tower);
            var skills = GetDamageSkills();
            foreach (var skill in skills)
                damages.Add(skill);
            return damages;
        }

        private ConfigEntityDamage GetDamageHero()
        {
            foreach (var entity in Damages)
            {
                if (entity.TypeEntity == TypeEntityStatisticDamage.Hero)
                return entity;
            }

            return null;
        }
        
        private ConfigEntityDamage GetDamageCastle()
        {
            foreach (var entity in Damages)
            {
                if (entity.TypeEntity == TypeEntityStatisticDamage.Castle)
                    return entity;
            }

            return null;
        }
        
        private List<ConfigEntityDamage> GetDamageTowers()
        {
            var list = new List<ConfigEntityDamage>();
            foreach (var entity in Damages)
            {
                if (entity.TypeEntity == TypeEntityStatisticDamage.Tower)
                    list.Add(entity);
            }
            list.Sort((a, b) => b.Damage.CompareTo(a.Damage));
            return list;
        }
        
        private List<ConfigEntityDamage> GetDamageSkills()
        {
            var list = new List<ConfigEntityDamage>();
            foreach (var entity in Damages)
            {
                if (entity.TypeEntity == TypeEntityStatisticDamage.Skill)
                    list.Add(entity);
            }
            list.Sort((a, b) => b.Damage.CompareTo(a.Damage));

            return list;
        }
    }
}
