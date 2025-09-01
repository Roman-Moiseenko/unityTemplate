using Game.Settings.Gameplay.Enemies;
using UnityEngine;

namespace Game.GamePlay.Services
{
    public static class GenerateService
    {

        public static MobParameters GenerateMobParameters(MobParameters baseParameters, int level)
        {
            if (level == 1) return baseParameters;
            
            var newParams = new MobParameters
            {
                Health = baseParameters.Health * ln(level),
                Attack = baseParameters.Attack * ln(level),
                Armor = (int)(baseParameters.Armor * ln(level, 1)),
                RewardCurrency = (int)(baseParameters.RewardCurrency * ln(level))
            };
            
            return newParams;
        }


        private static float ln(int index, int coef = 2)
        {
            var value = Mathf.Log(index) - Mathf.Log(index - 1);

            return value * coef + 1;
        }

        public static int GetWaveHealth(int numberWave)
        {
            const int baseWaveHealth = 760;
            return (int)(baseWaveHealth * ln(numberWave));
        }
    }
}