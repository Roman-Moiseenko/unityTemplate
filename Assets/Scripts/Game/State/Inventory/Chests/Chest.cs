using System;
using R3;
using UnityEngine;

namespace Game.State.Inventory.Chests
{
    public class Chest
    {
        public ChestEntityData Origin;

        public TypeChest TypeChest => Origin.TypeChest;
        public ReactiveProperty<long> TimeStart;
        
        public ReactiveProperty<bool> TimeOut = new(false);

        public ReactiveProperty<StatusChest> Status;

        //public int FullTimeOpening => Origin.TypeChest.TimeOut();
        public int Level => Origin.Level;

        public Chest(ChestEntityData chestEntityData)
        {
            Origin = chestEntityData;
            TimeStart = new ReactiveProperty<long>(chestEntityData.TimeStart);
            TimeStart.Subscribe(newValue => chestEntityData.TimeStart = newValue);
            Status = new ReactiveProperty<StatusChest>(chestEntityData.Status);
            Status.Subscribe(newValue => chestEntityData.Status = newValue);
        }

        public void StartOpenChest()
        {
            //TODO отследить сохранение Через Команду !!!!!! Команды старт открытия, ускорить открытие, открыть
            //
            //Origin.TimeStart = DateTime.Now.ToUniversalTime().ToFileTimeUtc();
        }

        /**
         * Сколько осталось до открытия сундука Vector2Int(часы, минуты)
         */
        public Vector2Int GetTimeOut()
        {
            var allTime = Origin.TypeChest.TimeOut() * 60;
            var time = Vector2Int.zero;
            if (Origin.TimeStart != 0)
            {
                var tNow = DateTime.Now.ToUniversalTime();
                var tStart = DateTime.FromFileTimeUtc(Origin.TimeStart);
                var span = tNow - tStart;
                var result = (int)span.TotalMinutes;

                allTime -= result;
                if (allTime < 0) allTime = 0;
            }
            time.x = allTime / 60;
            time.y = allTime % 60;
            
            return time;
        }

        public bool IsClosed()
        {
            return TimeStart.CurrentValue == 0;
        }
    }
}