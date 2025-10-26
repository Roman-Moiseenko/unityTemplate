using R3;

namespace Game.State.Root
{
    public class LoadingState
    {
        public ReactiveProperty<string> TextState = new();
        public bool Loaded = false;

        public void Clear()
        {
           // TextState.Value = "";
            Loaded = false;
        }

        public void Set(string state)
        {
            TextState.Value = state;
        }
    }
}