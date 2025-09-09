using System.Collections.Generic;
using System.Linq;
using Game.GameRoot.Services;
using Game.Settings;
using Game.Settings.Gameplay;
using Game.Settings.Gameplay.Enemies;
using MVVM.CMD;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.GamePlay.Commands.WaveCommands
{
    public class CommandWaveGenerateHandler : ICommandHandler<CommandWaveGenerate>
    {
        private readonly GameSettings _gameSettings;
        private readonly ICommandProcessor _cmd;
        private readonly GenerateService _generateService;
        private readonly InfinitySetting _infinitySetting;

        public CommandWaveGenerateHandler(GameSettings gameSettings, ICommandProcessor cmd, GenerateService generateService)
        {
            _gameSettings = gameSettings;
            _cmd = cmd;
            _generateService = generateService;
            _infinitySetting = _gameSettings.MapsSettings.InfinitySetting;
        }
        public bool Handle(CommandWaveGenerate command)
        {
            var commandWave = new CommandCreateWave
            {
                Index = command.Wave,
                WaveItems = GenerateWave(command.Wave),
            };
            _cmd.Process(commandWave);
            return false;
        }
        
        private List<WaveItemSettings> GenerateWave(int numberWave)
        {
            var list = new List<WaveItemSettings>();
            var levelMob = Mathf.Min((int)Mathf.Ceil(numberWave / _infinitySetting.rateLevelMob), 99); //Номинальный уровень моба на волну, не более 99
            var waveHealth = _generateService.GetLevelData(numberWave, _infinitySetting.ratioCurveWave) * 760; //Максимальная сила на волну
            if (numberWave % _infinitySetting.rateBoss == 0) waveHealth /= 2; //если босс, то пополам
            Debug.Log("numberWave = " + numberWave + " сила на волну = " + waveHealth);
            //Генерируем список мобов для волны
            var mobs = _gameSettings.MobsSettings.AllMobs.ToList();
            var mobsInWave = new Dictionary<string, float>();
            var bL = Mathf.RoundToInt(Mathf.Abs(Random.insideUnitSphere.x) + 1);
            var countTypeMob = Mathf.Min(numberWave / 10 + bL, mobs.Count, 5);
            if (countTypeMob == 5) //рандом на кол-во типов
                countTypeMob = Mathf.RoundToInt(Random.insideUnitSphere.x * 2) + 3;
            //Debug.Log("countTypeMob = " + countTypeMob);

            for (int i = 0; i < countTypeMob; i++)
            {
                var v = new System.Random();
                var index = v.Next(0, mobs.Count - 1);
                
               // var index = Mathf.RoundToInt(Mathf.Abs(Random.insideUnitSphere.x) * mobs.Count);
                var powerMob = (_generateService.GetLevelData(levelMob, _infinitySetting.ratioCurveMobs) * mobs[index].Health)
                               * (_generateService.GetLevelData(levelMob, _infinitySetting.ratioCurveMobs) * mobs[index].Attack) 
                               * mobs[index].SpeedMove * mobs[index].SpeedAttack / 9000 * _infinitySetting.ratioPower;
                Debug.Log(mobs[index].ConfigId + "  сила = " + powerMob);

                mobsInWave.Add(mobs[index].ConfigId, powerMob);
                mobs.Remove(mobs[index]);
            }
            
            var mediumPower = mobsInWave.Sum(f => f.Value);
            var allCount = Mathf.RoundToInt(waveHealth / mediumPower);
            Debug.Log("allCount = " + allCount);

            var _i = 0;
            
            foreach (var pairMobs in mobsInWave)
            {
                var mediumCount = allCount / (mobsInWave.Count - _i);
//                Debug.Log(mediumCount);
                var q = (int)Mathf.Abs(Random.insideUnitSphere.x) * mediumCount + 1;
                if (mediumCount == allCount) q = mediumCount;
                var setWave = new WaveItemSettings
                {
                    MobConfigId = pairMobs.Key,
                    Level = levelMob + (int)Mathf.Abs(Random.insideUnitSphere.x),
                    Quantity = q
                };
                list.Add(setWave);
                _i++;
            }
            
            if (numberWave % _infinitySetting.rateBoss == 0)
            {
                var bosses = _gameSettings.MobsSettings.AllBosses.ToList();
                var v = new System.Random();
                var indexBoss = v.Next(0, bosses.Count - 1);
                //Mathf.RoundToInt(Mathf.Abs(Random.insideUnitSphere.x) * bosses.Count) - 1;
                var setWave = new WaveItemSettings
                {
                    MobConfigId = bosses[indexBoss].ConfigId,
                    Level = numberWave / _infinitySetting.rateBoss,
                    Quantity = 1
                };
                list.Add(setWave);
                //TODO Random 2й босс, после 20 волны и тип босса разный
            }
//            Debug.Log("numberWave = " + numberWave);
            Debug.Log(JsonConvert.SerializeObject(list, Formatting.Indented));
            return list;
        }
    }
}