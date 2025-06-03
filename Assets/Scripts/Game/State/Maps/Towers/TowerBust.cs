namespace Game.State.Maps.Towers
{
    public class TowerBust
    {
        public int LevelBust; //0, 1, 2
        public TowerTypeBust MainBust;
        public TowerTypeBust SecondBust;
        public double MainAmount { get; set; }
        public double SecondAmount { get; set; }
        
    }
}