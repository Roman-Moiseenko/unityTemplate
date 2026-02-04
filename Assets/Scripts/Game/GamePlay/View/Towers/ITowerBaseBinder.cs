namespace Game.GamePlay.View.Towers
{
    public interface ITowerBaseBinder
    {
        void Bind(TowerViewModel viewModel);
        void DestroyGameObject();
    }
}