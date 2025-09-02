using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.GameRoot.Services
{
    public class GenerateService
    {
        private const int Rate = 2;
        
        public readonly Dictionary<int, float> LevelCoefs = new();

        public Dictionary<string, Dictionary<int, float>> Cache = new();

        public GenerateService()
        {
            
            LevelCoefs.Add(1, 1f);
            UpdateLevelCoefs(10);
            /*
            for (var i = 2; i < 1000; i++)
            {
                var value = Mathf.Log(i) - Mathf.Log(i - 1);
                LevelCoefs[i] = LevelCoefs[i - 1] * (value * Rate + 1);
            }*/
            //обсчет коэфициентов .. В дальнейшем загружать с сервера
          //  Debug.Log(JsonConvert.SerializeObject(LevelCoefs, Formatting.Indented));
        }


        public float GetLevelData(int level)
        {
            if (level > LevelCoefs.Count) UpdateLevelCoefs(level);
            
            return LevelCoefs[level];
        }

        private void UpdateLevelCoefs(int newLevel)
        {
            var beginIndex = LevelCoefs.Count + 1;
            for (var i = beginIndex; i <= newLevel; i++)
            {
                var value = Mathf.Log(i) - Mathf.Log(i - 1);
                LevelCoefs[i] = LevelCoefs[i - 1] * (value * Rate + 1);
            }
        }
/* 
        private float ln(int index)
        {
            if (index == 1) return 1;

            var value = Mathf.Log(index) - Mathf.Log(index - 1);

            return value * Rate + 1;
        }


        public float GetData(string config, int level, float baseData)
        {
            if (Cache.TryGetValue(config, out var floats))
            {
                if (floats.TryGetValue(level, out var value)) return value;

                var beginIndex = floats.Count + 1;
                UpdateLevel(floats, level, beginIndex);
 
                return floats[level];
            }

            var newFloats = new Dictionary<int, float>();
            newFloats.Add(1, baseData);
            UpdateLevel(newFloats, level, 2);

            Cache.Add(config, newFloats);
            return newFloats[level];
        }

       private void UpdateLevel(Dictionary<int, float> array, int level, int beginIndex)
        {
            for (var i = beginIndex; i <= level; i++)
            {
                var newValue = array[i - 1] * _lns[i];
                array.Add(i, newValue);
            }
        }*/
    }
}