using System;
using System.Collections.Generic;
using System.Linq;
using DI;
using Game.Common;
using Game.GamePlay.Commands.WaveCommands;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.GameplayStates;
using Game.GamePlay.Root;
using Game.GameRoot.Services;
using Game.MainMenu.Root;
using Game.Settings.Gameplay.Maps;
using Game.State.Inventory.Chests;
using Game.State.Maps.Rewards;
using Game.State.Root;
using MVVM.CMD;
using Newtonsoft.Json;
using R3;
using Debug = UnityEngine.Debug;
using Random = System.Random;

namespace Game.GamePlay.Services
{
    /**
     * Сервис отслеживающий события приводящие к концу игры, и высчитывающий выходные данные (награду)
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
        public ReactiveProperty<GameplayExitParams> GameOver = new(null);
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
            waveService.IsMobsOnWay.Where(x => !x).Subscribe(_ =>
                {
                    //Мобы на дороге закончились, проверяем закончились ли волны. 
                    if (waveService.FinishAllWaves.Value) Win();
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

        public void Win() //private
        {
            //Debug.Log("Победа");
            var menuParams = GetMainMenuParams(true);
            if (_gameState.ContainerChests.IsFreeCell())
            {
                menuParams.TypeChest = GetTypeChestWin(out var rewardChest);
                menuParams.LastRewardChest = rewardChest;
            }
            else
            {
                menuParams.TypeChest = TypeChest.None;
            }

            var exitParams = new GameplayExitParams(menuParams);
            //Debug.Log(JsonConvert.SerializeObject(exitParams, Formatting.Indented));

            GameOver.OnNext(exitParams);
            //TODO Показать окно окончания игры без сохранения
            // _exitSceneRequest.OnNext(exitParams); //TODO перенести в окно Finish
        }

        public void Lose() //private
        {
            var menuParams = GetMainMenuParams(false);
            if (_gameState.ContainerChests.IsFreeCell())
            {
                menuParams.TypeChest = GetTypeChestLose(menuParams.LastWave, out var rewardChest);
                menuParams.LastRewardChest = rewardChest;
            }
            else
            {
                menuParams.TypeChest = TypeChest.None;
            }

            var exitParams = new GameplayExitParams(menuParams);
            GameOver.OnNext(exitParams);
            //TODO Показать окно окончания игры
            _exitSceneRequest.OnNext(exitParams);
        }

        private TypeChest GetTypeChestLose(int lastWave, out TypeChest lastRewardChest)
        {
            lastRewardChest = TypeChest.Silver;
            if (_gameState.MapStates.Maps.TryGetValue(_gameplayState.MapId.CurrentValue, out var mapState))
            {
                var mapFinished = mapState.Finished.CurrentValue;
                lastRewardChest = mapState.RewardChest.CurrentValue;
                if (mapFinished) return TypeChest.Silver;
            }

            var map = _mapsSettings.Maps.Find(v => v.MapId == _gameplayState.MapId.CurrentValue);
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

        private TypeChest GetTypeChestWin(out TypeChest lastRewardChest)
        {
            //  lastRewardChest = null;
            var map = _mapsSettings.Maps.Find(v => v.MapId == _gameplayState.MapId.CurrentValue);
            var rewardChests = map.MapRewardSetting.RewardChest;
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
            //Награды в настройках
            var rewardSettings = _mapsSettings.Maps.Find(v => v.MapId == _gameplayState.MapId.CurrentValue);

            var result = new List<RewardEntityData>();
            lastRewardOnWave = 0;

            var completedWave = completedLevel
                ? rewardSettings.InitialStateSettings.Waves.Count()
                : _gameplayState.CurrentWave.CurrentValue - 1;

            // Debug.Log(completedLevel +" == " + completedWave);
            var rewardWave = 0; //Последняя полученная награда за волну По-умолчанию
            if (_gameState.MapStates.Maps.TryGetValue(_gameplayState.MapId.CurrentValue, out var mapState))
            {
                //Debug.Log(JsonConvert.SerializeObject(mapState));
                //Уже получали награды
                rewardWave = mapState.RewardOnWave.CurrentValue;
                lastRewardOnWave = rewardWave;
                if (completedWave < rewardWave) return null; //Наград больше нет, все получено ранее 
            }

            //Debug.Log(JsonConvert.SerializeObject(rewardSettings.MapRewardSetting, Formatting.Indented));
            //if (rewardSettings == null) return null;

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
                            .Find(v => v.RewardType == rewardItem.RewardType && v.ConfigId == rewardItem.ConfigId);
                        if (rewardInResult != null)
                        {
                            rewardInResult.Amount += rewardItem.Amount;
                        }
                        else
                        {
                            result.Add(new RewardEntityData
                            {
                                ConfigId = rewardItem.ConfigId,
                                RewardType = rewardItem.RewardType,
                                Amount = rewardItem.Amount
                            });
                        }
                    }
                }
            }

            // Debug.Log(JsonConvert.SerializeObject(result, Formatting.Indented));
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
            menuParams.FinishedMap = completedLevel;
            menuParams.LastWave = completedLevel
                ? _gameplayState.CurrentWave.CurrentValue
                : _gameplayState.CurrentWave.CurrentValue - 1;

            menuParams.KillsMob = _gameplayState.KillMobs.CurrentValue;
            menuParams.TypeGameplay = _gameplayState.Origin.TypeGameplay;

            menuParams.RewardOnWave = GetRewardOnWave(completedLevel, out var lastRewardOnWave);
            menuParams.LastRewardOnWave = lastRewardOnWave;

            return menuParams;
        }

        /**
         * Прервать игру
         */
        public void Abort()
        {
            var exitParams = new GameplayExitParams(null);
            GameOver.OnNext(exitParams);
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
            GameOver.OnNext(exitParams);
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