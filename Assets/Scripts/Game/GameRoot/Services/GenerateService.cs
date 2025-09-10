using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.GameRoot.Services
{
    public class GenerateService
    {
        private const float BaseRate = 2f;
        private readonly Dictionary<float, Dictionary<int, float>> _ratioCurve = new();
        
        public GenerateService()
        {
            var d = new Dictionary<int, float> { { 1, 1f } };
            _ratioCurve.Add(BaseRate, d);
            UpdateRatioCurve(BaseRate, 10);
        }

        private void UpdateRatioCurve(float baseRate, int newLevel)
        {
            if (_ratioCurve.TryGetValue(baseRate, out var listRatio))
            {
                if (listRatio.Count >= newLevel) return;
                
                var beginIndex = listRatio.Count + 1;
                for (var i = beginIndex; i <= newLevel; i++)
                {
                    var value = Mathf.Log(i) - Mathf.Log(i - 1);
                    listRatio[i] = listRatio[i - 1] * (value * baseRate + 1);
                }
            }
            else
            {
                throw new Exception("Исключительная ситуация");
            }
        }

        public float GetRatioCurve(int level, float ratio)
        {
            if (_ratioCurve.TryGetValue(ratio, out var listRatio))
            {
                if (level > listRatio.Count) UpdateRatioCurve(ratio, level);
                
            }
            else
            {
                var d = new Dictionary<int, float> { { 1, 1f } };
                _ratioCurve.Add(ratio, d);
                UpdateRatioCurve(ratio, level);
            }
            return _ratioCurve[ratio][level];
        }
        

    }
}