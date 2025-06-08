namespace Game.State.Gameplay
{
    public class RewardCardData
    {
        public int Position;
        public RewardType RewardType;
        public string TargetId; //Цель Конфиг башни
        public int RewardLevel; //Уровень наград
        public string ConfigId; //Конфиг бустера ()


        public bool IsBuild()
        {
            if (this.RewardType == RewardType.Ground || this.RewardType == RewardType.Tower ||
                this.RewardType == RewardType.Road)
                return true;
            return false;
        }
    }
}