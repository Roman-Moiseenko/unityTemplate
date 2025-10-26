namespace Game.Settings.Gameplay
{
    public class InfinitySettingWeb
    {
        public float ratioCurveWave = 2;//Коэф.роста сложности волны от ее номера
        public float ratioCurveMobs = 2; //Коэф.роста сложности мобов от уровня
        public float rateLevelMob = 10f; //Через сколько волн увеличивается лвл мобов
        public int rateBoss = 10; //Частота появления босса
        public float ratioPower = 1f; //Усиление мобов
        public int rateRewardEntity = 5; //Через сколько волн выдается награда карточкой или чертежом
    }
}