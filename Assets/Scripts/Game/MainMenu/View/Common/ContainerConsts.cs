namespace Game.MainMenu.View.Common
{
    public class ContainerConsts
    {
        public int Height {get; set;}
        public int Spacing {get; set;}
        public int MinHeight {get; set;}
        public int Cols {get; set;}

        public ContainerConsts(int height, int spacing, int minHeight, int cols)
        {
            Height = height;
            Spacing = spacing;
            MinHeight = minHeight;
            Cols = cols;
        }
    }
}