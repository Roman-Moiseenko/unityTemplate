using R3;
using UnityEngine;
using UnityEngine.VFX;

namespace Game.GamePlay.View.Hero
{
    public class HeroBinder : MonoBehaviour
    {
        [SerializeField] private Transform heroContainer;
        [SerializeField] protected VisualEffect levelUp;
        [SerializeField] private Animator animator;
        [SerializeField] private HeroVisibleBinder visibleBinder;
        [SerializeField] private HeroGunBinder gun;
        
        private HeroViewModel _viewModel;
        private DisposableBag _disposables;
        
        
        private const string AnimationAwait = "hero_await";
        private const string AnimationMoving = "hero_moving";
        private const string AnimationAttack = "hero_attack";
        

        public void Bind(HeroViewModel viewModel)
        {
            _viewModel = viewModel;
            visibleBinder.Bind(viewModel);
            gun.Bind(viewModel);
            
            transform.position = new Vector3(
                viewModel.Position.CurrentValue.x, 0, viewModel.Position.CurrentValue.y);
        }

        //MAINDO Обработка выстрела, включение анимаций

        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}