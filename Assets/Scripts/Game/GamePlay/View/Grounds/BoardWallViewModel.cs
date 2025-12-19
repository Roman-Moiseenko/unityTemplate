namespace Game.GamePlay.View.Grounds
{
    public class BoardWallViewModel
    {
        public string Id;
        public string ConfigId;
        public int Rotation = 0;
        public float DeltaX = 0;
        public float DeltaY = 0;
        public bool ShowLeftSide = true;
        public bool ShowRightSide = true;

        public BoardWallViewModel()
        {
            
        }

    }
}