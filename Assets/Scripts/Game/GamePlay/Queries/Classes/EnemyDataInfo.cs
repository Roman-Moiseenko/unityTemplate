using Game.State.Maps.Mobs;

namespace Game.GamePlay.Queries.Classes
{
    /**
     * Класс для передачи данных из настроек в Окно Сведения о Волне
     */
    public class EnemyDataInfo
    {
        public string ConfigId;
        public string TitleLid;
        public int Quantity;
        public MobDefence Defence;
        public bool IsBoss = false;
    }
}