namespace Game.State.Root
{
    public class LoadingState
    {
        public string TextState = "";
        public bool Loaded = false;

        public void Clear()
        {
            TextState = "";
            Loaded = false;
        }
    }
}