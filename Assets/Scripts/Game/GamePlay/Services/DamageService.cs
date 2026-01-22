using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.GamePlay.Fsm;
using Game.State.Maps.Mobs;
using Game.State.Maps.Shots;
using Game.State.Maps.Towers;
using Game.State.Maps.Warriors;
using Game.State.Root;
using ObservableCollections;
using R3;
using Scripts.Utils;
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
            GameplayStateProxy gameplayState,
            WaveService waveService,
            RewardProgressService rewardProgressService
        )
        {
            gameplayState.Shots.ObserveAdd().Subscribe(e =>
            {
                var shot = e.Value;

                //Ищем моба, переделать на gameplayState.Mobs и перенести MobsEntity в gameplayState
                if (!waveService.AllMobsMap.TryGetValue(shot.MobEntityId, out var mobEntity))
                {
                    gameplayState.Shots.Remove(shot); //Сущность уже удалена
                    return;
                }

                if (shot.Single) //Одиночный урон
                {
                    SetDamageMob(mobEntity, shot);
                    gameplayState.Shots.Remove(shot);
                    return;
                }

                //Найти всех мобов в радиусе поражения и нанести каждому урон
                var position = mobEntity.Position.CurrentValue;
                if (shot.DamageType == DamageType.Normal) shot.DamageType = DamageType.MassDamage;

                var mobsUnderAttacks = new List<MobEntity>();
                //Ищем соучастников урона и с проверкой на совместимость воздух/земля
                foreach (var (k, entity) in waveService.AllMobsMap)
                {
                    if (Vector2.Distance(position, entity.GetPosition()) <= 0.5f &&
                        entity.IsFly == shot.IsFly) mobsUnderAttacks.Add(entity);
                }

                foreach (var mobUnderAttack in mobsUnderAttacks)
                {
                    shot.Position = mobUnderAttack.GetPosition();
                    SetDamageMob(mobUnderAttack, shot);
                }

                gameplayState.Shots.Remove(shot); //Удаляем из списка выстрел
            });

            //TODO Перенести в waveService или rewardProgressService(!)
            waveService.AllMobsMap.ObserveAdd().Subscribe(e =>
            {
                var mobEntity = e.Value.Value;
                //Проверяем, что моб мертв 
                mobEntity.IsDead.Skip(1)
                    .Where(x => x)
                    .Subscribe(_ =>
                        {
                            //выдаем награды и другое 
                            rewardProgressService.RewardKillMob(mobEntity.RewardCurrency,
                                mobEntity.Position.CurrentValue);
                            waveService.AllMobsMap.Remove(e.Value.Key);
                        }
                    );
            });
        }

        //TODO Переделать под универсальную функцию
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