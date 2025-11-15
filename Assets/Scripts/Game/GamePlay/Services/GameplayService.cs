using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DI;
using Game.Common;
using Game.GamePlay.Classes;
using Game.GamePlay.Commands.WaveCommands;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.GameplayStates;
using Game.GamePlay.Root;
using Game.GameRoot.Services;
using Game.MainMenu.Root;
using Game.Settings.Gameplay.Maps;
using Game.State;
using Game.State.GameStates;
using Game.State.Inventory;
using Game.State.Inventory.Chests;
using Game.State.Maps.Mobs;
using Game.State.Maps.Rewards;
using Game.State.Root;
using MVVM.CMD;
using Newtonsoft.Json;
using R3;
using Scripts.Game.GameRoot.Entity;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = System.Random;

namespace Game.GamePlay.Services
{
    /**
     * Сервис для подписок на события которые необходимо сохранить.
     */
    public class GameplayService : IDisposable
    {
        private readonly Subject<GameplayExitParams> _exitSceneRequest;
        private readonly DIContainer _container;
        private readonly GameplayStateProxy _gameplayState;
        private readonly AdService _adService;
        private WaveService _waveService;
        private FsmGameplay _fsmGameplay;
        private readonly ResourceService _resourceService;
        private readonly ICommandProcessor _cmd;
        private readonly GameStateProxy _gameState;
        private readonly MapsSettings _mapsSettings;

        public int CountRepair;

        private IDisposable _disposable;
        // public ReactiveProperty<float> CastleHealth;

        public GameplayService(
            Subject<GameplayExitParams> exitSceneRequest,
            WaveService waveService,
            GameplayStateProxy gameplayState,
            AdService adService,
            FsmGameplay fsmGameplay,
            ResourceService resourceService,
            ICommandProcessor cmd,
            GameStateProxy gameState,
            MapsSettings mapsSettings
        )
        {
            var d = Disposable.CreateBuilder();
            CountRepair = 0;
            _exitSceneRequest = exitSceneRequest;
            _gameplayState = gameplayState;
            _adService = adService;
            _fsmGameplay = fsmGameplay;
            _waveService = waveService;
            _resourceService = resourceService;
            _cmd = cmd;
            _gameState = gameState;
            _mapsSettings = mapsSettings;

            //TODO Переделать на другой параметр
            waveService.IsMobsOnWay.Where(flag => flag == false).Subscribe(_ =>
                {
                    //Мобы на дороге закончились, проверяем закончились ли волны. 
                    if (_gameplayState.IsFinishWaves()) Win();
                }
            ).AddTo(ref d);

            //Для бесконечной игры добавляем автоувеличение уровня
            if (_gameplayState.IsInfinity())
            {
                _gameplayState.CurrentWave.Skip(1).Where(v => v == _gameplayState.Waves.Count).Subscribe(v =>
                {
                    var command = new CommandWaveGenerate(v + 1);
                    cmd.Process(command);
                }).AddTo(ref d);
            }

            _gameplayState.Castle.IsDead.Where(e => e)
                .Subscribe(newValue =>
                {
                    if (_gameplayState.Castle.CountResurrection.CurrentValue == 2)
                    {
                        Lose();
                    }
                })
                .AddTo(ref d);
            /*   _exitSceneRequest.Subscribe(e =>
               {
                   Debug.Log(JsonConvert.SerializeObject(e.MainMenuEnterParams, Formatting.Indented));
               });*/
            //Сработала следующая волна, после максимальной => Победа

            //Здоровье крепости меньше 0 => Проигрыш
            //gameplayState.Castle.IsDead.Subscribe(h => Lose()).AddTo(ref d);
            _disposable = d.Build();
        }

        //TODO Расчитать награду - сундук и др
        public void Win() //private
        {
            var menuParams = GetMainMenuParams(true);
            menuParams.TypeChest = GetTypeChestWin(out var rewardChest);
            menuParams.LastRewardChest = rewardChest;
            
            var exitParams = new GameplayExitParams(menuParams);
            _exitSceneRequest.OnNext(exitParams);
        }

        public void Lose() //private
        {
            var menuParams = GetMainMenuParams(false);
            menuParams.TypeChest = GetTypeChestLose(menuParams.LastWave, out var rewardChest);
            menuParams.LastRewardChest = rewardChest;

            var exitParams = new GameplayExitParams(menuParams);
            _exitSceneRequest.OnNext(exitParams);
        }

        private TypeChest? GetTypeChestLose(int lastWave, out TypeChest lastRewardChest)
        {
            lastRewardChest = TypeChest.Silver;
            if (_gameState.MapStates.Maps.TryGetValue(_gameplayState.MapId.CurrentValue, out var mapState))
            {
                var mapFinished = mapState.Finished.CurrentValue;
                lastRewardChest = mapState.RewardChest.CurrentValue;
                if (mapFinished) return TypeChest.Silver;
            }

            var map = _mapsSettings.Maps[_gameplayState.MapId.CurrentValue];
            var rewardChests = map.MapRewardSetting.RewardChest;
            var maxWave = map.InitialStateSettings.Waves.Count;
 
            var coef = lastWave * 1.0f / maxWave;
            
            if (rewardChests.Count == 3 && coef > 0.5f && lastRewardChest == TypeChest.Silver)
            {
                lastRewardChest = TypeChest.Gold;
                return TypeChest.Gold;
            }

            if (rewardChests.Count == 4)
            {
                if (coef is > 0.33f and < 0.66f && lastRewardChest == TypeChest.Silver)
                {
                    lastRewardChest = TypeChest.Gold;
                    return TypeChest.Gold;
                }
                
                if (coef >= 0.66f && lastRewardChest != TypeChest.Epic)
                {
                    lastRewardChest = TypeChest.Epic;
                    return TypeChest.Epic;
                }
            }
            return TypeChest.Silver;
        }

        private TypeChest GetTypeChestWin(out TypeChest? lastRewardChest)
        {
          //  lastRewardChest = null;
            var rewardChests = _mapsSettings.Maps[_gameplayState.MapId.CurrentValue].MapRewardSetting.RewardChest;
            var maxChest = TypeChest.Silver;
            var maxValueRandom = 0;
            foreach (var (type, rewardItems) in rewardChests)
            {
                if (type.GetIndex() > maxChest.GetIndex())
                {
                    maxChest = type;
                    maxValueRandom += type.GetRandom();
                }
            }

            lastRewardChest = maxChest;
            if (_gameState.MapStates.Maps.TryGetValue(_gameplayState.MapId.CurrentValue, out var mapState))
            {
                if (mapState.RewardChest.CurrentValue == maxChest)
                {
                    var random = new Random();
                    var keyRandom = random.Next(maxValueRandom);
                    foreach (var (type, rewardItems) in rewardChests)
                    {
                        keyRandom -= type.GetRandom();
                        if (keyRandom <= 0) return type;
                    }
                    
                }
            }

            return maxChest;
        }

        private List<RewardEntityData> GetRewardOnWave(bool completedLevel, out int lastRewardOnWave)
        {
            var result = new List<RewardEntityData>();
            lastRewardOnWave = 0;

            var completedWave = completedLevel
                ? _gameplayState.CurrentWave.CurrentValue
                : _gameplayState.CurrentWave.CurrentValue - 1;
            var rewardWave = 0; //Последняя полученная награда за волну По-умолчанию
            if (_gameState.MapStates.Maps.TryGetValue(_gameplayState.MapId.CurrentValue, out var mapState))
            {
                //Уже получали награды
                rewardWave = mapState.RewardOnWave.CurrentValue;
                lastRewardOnWave = rewardWave;
                if (completedWave < rewardWave) return null; //Наград больше нет, все получено ранее 
            }

            //Награды в настройках
            var rewardSettings = _mapsSettings.Maps.Find(v => v.MapId == _gameplayState.MapId.CurrentValue);
            if (rewardSettings == null) return null;

            foreach (var (wave, list) in rewardSettings.MapRewardSetting.RewardOnWave)
            {
                //тек.волна > волны награды, которую еще не получили
                if (completedWave >= wave && rewardWave < wave)
                {
                    lastRewardOnWave = wave;
                    //Получить все награды по всем волнам неполученным
                    foreach (var rewardItem in list)
                    {
                        var rewardInResult = result
                            .Find(v => v.RewardType == rewardItem.Type && v.ConfigId == rewardItem.ConfigId);
                        if (rewardInResult != null)
                        {
                            rewardInResult.Amount += rewardItem.Amount;
                        }
                        else
                        {
                            result.Add(new RewardEntityData
                            {
                                ConfigId = rewardItem.ConfigId,
                                RewardType = rewardItem.Type,
                                Amount = rewardItem.Amount
                            });
                        }
                    }
                }
            }

            return result;
        }

        private MainMenuEnterParams GetMainMenuParams(bool completedLevel)
        {
            var menuParams = new MainMenuEnterParams("Выход");
            //Базовые награды
            menuParams.GameSpeed = _gameplayState.GetLastSpeedGame();
            menuParams.SoftCurrency = _gameplayState.SoftCurrency.CurrentValue;
            menuParams.MapId = _gameplayState.MapId.CurrentValue;
            menuParams.RewardCards = _gameplayState.Origin.RewardEntities;
            menuParams.CompletedLevel = completedLevel;
            menuParams.LastWave = completedLevel
                ? _gameplayState.CurrentWave.CurrentValue
                : _gameplayState.CurrentWave.CurrentValue - 1;

            menuParams.KillsMob = _gameplayState.KillMobs.CurrentValue;
            menuParams.TypeGameplay = _gameplayState.Origin.TypeGameplay;

            menuParams.RewardOnWave = GetRewardOnWave(false, out var lastRewardOnWave);
            menuParams.LastRewardOnWave = lastRewardOnWave;


            return menuParams;
        }

        /**
         * Прервать игру
         */
        public void Abort()
        {
            var exitParams = new GameplayExitParams(null);
            _exitSceneRequest.OnNext(exitParams);
        }

        /**
         * Для тестирования
         */
        public void ExitSave()
        {
            var menuParams = GetMainMenuParams(false);
            var exitParams = new GameplayExitParams(menuParams);
            exitParams.SaveGameplay = true;
            _exitSceneRequest.OnNext(exitParams);
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }

        public void RepairCristal()
        {
            //TODO Тратим кристалы, если нет PopupError
            if (_resourceService.SpendHardCurrency(AppConstants.COST_REPAIR_CASTLE)) Repair();
        }

        public void RepairAd()
        {
            //TODO 

            var ad = _adService.ShowAdGoogle();
            _fsmGameplay.Fsm.SetState<FsmStateGamePause>();

            ad.CloseShow.Subscribe(v =>
            {
                if (v.Success.CurrentValue)
                {
                    Repair();
                    _fsmGameplay.Fsm.SetState<FsmStateGamePlay>();
                }
                else
                {
                    Lose();
                }
            });
        }

        private void Repair()
        {
            if (!_gameplayState.Castle.Resurrection()) return;

            foreach (var entity in _waveService.AllMobsMap.ToArray())
            {
                entity.Value.SetDamage(_gameplayState.Castle.FullHealth);
            }
        }
    }
}