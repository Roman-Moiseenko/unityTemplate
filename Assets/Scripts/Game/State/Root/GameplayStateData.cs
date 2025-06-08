namespace Game.State.Root
{
    public class GameplayStateData
    {
        public int GameSpeed { get; set; }
        public int PreviousGameSpeed { get; set; }
        
        public int Progress { get; set; } //Текущий прогресс игры, растет от убийства мобов, при = наполнении обнуляем и увеличиваем ProgressLevel на 1
        public int ProgressLevel { get; set; } //Влияет на коэффициент роста Progress в обратно-пропорциональном порядке 
        
    }
}