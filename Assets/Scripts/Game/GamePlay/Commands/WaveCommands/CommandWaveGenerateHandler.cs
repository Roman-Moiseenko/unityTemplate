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
            var random = new System.Random();
            var list = new List<WaveItemSettings>();
            var levelMob = Mathf.Min((int)Mathf.Ceil(numberWave / _infinitySetting.rateLevelMob), 99); //Номинальный уровень моба на волну, не более 99
            var waveHealth = _generateService.GetRatioCurve(numberWave, _infinitySetting.ratioCurveWave) * 760; //Максимальная сила на волну
            if (numberWave % _infinitySetting.rateBoss == 0) waveHealth /= 2; //если босс, то пополам
         //   Debug.Log("numberWave = " + numberWave + " сила на волну = " + waveHealth);
            //Генерируем список мобов для волны
            var mobs = _gameSettings.MobsSettings.AllMobs.ToList();
            var mobsInWave = new Dictionary<string, float>();
            
            var countTypeMob = 0;
            if (numberWave < 5) countTypeMob = random.Next(1, 3); // 1 или 2
            if (numberWave is >= 5 and <= 10 ) countTypeMob = random.Next(2, 4);
            if (numberWave > 10) countTypeMob = random.Next(3, 6);            
            
            //Debug.Log("countTypeMob = " + countTypeMob);

            for (int i = 0; i < countTypeMob; i++)
            {
                var v = new System.Random();
                var index = v.Next(0, mobs.Count);
                
               // var index = Mathf.RoundToInt(Mathf.Abs(Random.insideUnitSphere.x) * mobs.Count);
                var powerMob = (_generateService.GetRatioCurve(levelMob, _infinitySetting.ratioCurveMobs) * mobs[index].Health)
                               * mobs[index].Attack 
                               * mobs[index].SpeedMove * mobs[index].SpeedAttack / 9000 * _infinitySetting.ratioPower;
                //Debug.Log(mobs[index].ConfigId + "  сила = " + powerMob);

                mobsInWave.Add(mobs[index].ConfigId, powerMob);
                mobs.Remove(mobs[index]);
            }
            
            var mediumPower = mobsInWave.Sum(f => f.Value);
            var allCount = Mathf.RoundToInt(waveHealth / mediumPower);
           // Debug.Log("allCount = " + allCount);

            var _i = 0;
            
          //  Debug.Log(" mobsInWave.Count = " + mobsInWave.Count);
            foreach (var pairMobs in mobsInWave)
            {
                var mediumCount = allCount / (mobsInWave.Count - _i);
              //  Debug.Log(allCount + " medium = " + mediumCount);
                var q = random.Next(1, mediumCount + 1);// (int)Mathf.Abs(Random.insideUnitSphere.x) * mediumCount + 1;
                

                if (mediumCount == allCount) q = mediumCount;
             //   Debug.Log(pairMobs.Key + " - " + q);
                
                var setWave = new WaveItemSettings
                {
                    MobConfigId = pairMobs.Key,
                    Level = levelMob + (int)Mathf.Abs(Random.insideUnitSphere.x),
                    Quantity = q
                };
                allCount -= q;
                list.Add(setWave);
                _i++;
            }
            
            if (numberWave % _infinitySetting.rateBoss == 0)
            {
                var bosses = _gameSettings.MobsSettings.AllBosses.ToList();
                var v = new System.Random();
                var indexBoss = v.Next(0, bosses.Count);
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
         //   Debug.Log(JsonConvert.SerializeObject(list, Formatting.Indented));
            return list;
        }
    }
}