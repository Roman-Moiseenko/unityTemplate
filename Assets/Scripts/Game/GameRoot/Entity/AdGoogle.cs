using R3;

namespace Scripts.Game.GameRoot.Entity
{
    public class AdGoogle
    {
        public Observable<AdGoogle> CloseShow => _closeShow; //На событие закрытия можно подписаться извне

        private readonly Subject<AdGoogle> _closeShow = new(); //Создаем событие для закрытия окна

        public ReactiveProperty<bool> Success;

        public AdGoogle()
        {
            Success = new ReactiveProperty<bool>(false);
        }
        public void SuccessShow()
        {
            Success.Value = true;
            _closeShow.OnNext(this);
        }
        
        public void AbortShow()
        {
            Success.Value = false;
            _closeShow.OnNext(this);
        }
        
    }
}