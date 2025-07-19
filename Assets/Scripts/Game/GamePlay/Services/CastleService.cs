
using System.Collections.Generic;
using Game.GamePlay.Commands;
using Game.GamePlay.View.Buildings;
using Game.GamePlay.View.Castle;
using Game.Settings.Gameplay.Buildings;
using Game.Settings.Gameplay.Entities.Buildings;
using Game.State.Entities;
using Game.State.Maps.Castle;
using Game.State.Mergeable.Buildings;
using MVVM.CMD;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.Services
{
    public class CastleService
    {
        private readonly ICommandProcessor _cmd;

  //      private readonly ObservableList<BuildingViewModel> _allBuildings = new();
     //   private readonly Dictionary<int, BuildingViewModel> _buildingsMap = new();
      //  private readonly Dictionary<string, BuildingSettings> _buildingSettingsMap = new();
      //  public IObservableCollection<BuildingViewModel> AllBuildings => _allBuildings; //Интерфейс менять нельзя, возвращаем через динамический массив
      public CastleViewModel CastleViewModel { get; }

      /**
         * При загрузке создаем все view-модели из реактивного списка всех строений 
         * Подписываемся на событие добавления в массив Proxy сущностей
        */
         
        public CastleService(
            CastleEntity castleEntity,
            WaveService waveService
            )
        {
            
            CastleViewModel = new CastleViewModel(castleEntity, this);
            
        }
        
    }
}
