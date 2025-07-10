using System.Collections;
using DI;
using Game.GamePlay.View.Towers;
using Game.State.Maps.Shots;
using Game.State.Maps.Towers;
using Game.State.Root;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.Services
{
    public class ShotService
    {
        private readonly GameplayStateProxy _gameplayState;

        public ObservableList<ShotEntity> Shots = new();

        //TODO Нужны ли данные в конструктор? Возможно настройки базовые?
        public ShotService(GameplayStateProxy gameplayState)
        {
            _gameplayState = gameplayState;
        }


        public void CreateShot()
        {
            
        }


        
        
    }
}