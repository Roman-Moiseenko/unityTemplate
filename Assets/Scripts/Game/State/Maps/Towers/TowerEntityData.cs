using System.Collections.Generic;
using Game.State.Entities;

namespace Game.State.Maps.Towers
{
    public class TowerEntityData : EntityData
    {
        public TowerTypeDamage TypeDamage { get; set; }
        public TowerTypeEnemy TypeEnemy { get; set; }
        public int EpicLevel { get; set; }
        //Список усилений, навешанных на башню
        public List<TowerBust> Busts = new();

        //TODO Базовые данные.
        ///Урон,
        /// Скорость атаки,
        /// радиус поражения,
        /// пост урон,
        /// продолжительность пост урона
        /// 
        public double Damage { get; set; }
        public double Speed { get; set; }
        

    }
}