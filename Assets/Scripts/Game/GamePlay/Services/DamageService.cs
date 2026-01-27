using System.Linq;
using Game.State.Maps.Mobs;
using Game.State.Maps.Shots;
using Game.State.Root;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.Services
{
    /**
     * Сервис нанесения урона мобам по выстрелам gameplayState.Shots, также проверка на урон по площади
     * запуска награды и создания списка для отображения popup урона
     */
    public class DamageService
    {
        public ObservableList<DamageEntity> AllDamages = new();

        public DamageService(
            GameplayStateProxy gameplayState
        )
        {
            gameplayState.Shots.ObserveAdd().Subscribe(e =>
            {
                var shot = e.Value;
                MobEntity mobEntity = gameplayState.Mobs.FirstOrDefault(mob => mob.UniqueId == shot.MobEntityId);
                //Ищем моба, переделать на gameplayState.Mobs и перенести MobsEntity в gameplayState
                if (mobEntity != null) SetDamageMob(mobEntity, shot);
                gameplayState.Shots.Remove(shot);
            });
        }

        public void SetDamageMob(MobEntity mobEntity, ShotData shot)
        {
            var damage = new DamageEntity
            {
                Position = mobEntity.Position.CurrentValue,
                Damage = Mathf.FloorToInt(mobEntity.SetDamage(shot.Damage)),
                Type = shot.DamageType,
            };
            AllDamages.Add(damage);
            //Устанавливаем дебаф, если есть
            if (shot.Debuff != null) mobEntity.SetDebuff(shot.ConfigId, shot.Debuff);
            //TODO если есть продолжительный урон, добавляем мобу mobEntity.PeriodDamage(shot.ConfigId, shot.PeriodDamage)
        }
    }
}