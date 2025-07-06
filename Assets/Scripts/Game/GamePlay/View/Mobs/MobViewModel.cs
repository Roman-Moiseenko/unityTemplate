using Game.State.Maps.Mobs;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Mobs
{
    public class MobViewModel
    {
        private MobEntity _mobEntity;
        public int MobEntityId => _mobEntity.UniqueId;
        public string ConfigId => _mobEntity.ConfigId;

      //  public ReactiveProperty<Vector3> Position;
      //  public ReactiveProperty<Vector2> Direction;

        public MobViewModel(MobEntity mobEntity)
        {
            _mobEntity = mobEntity;
            Debug.Log("Создаем View Model для " + ConfigId);
            
            //TODO Заполняем данными модель
        }
    }
}