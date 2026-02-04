using DG.Tweening;
using MVVM.Storage;
using R3;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.GamePlay.View.UI.ScreenGameplay.Rewards
{
    public class CurrencyPopupBinder : MonoBehaviour, IPoolElement
    {
        private Camera _camera;

        private Sequence Sequence { get; set; }

        public void Bind()
        {
            _camera = Camera.main;
        }
        
        public ReactiveProperty<bool> StartPopup(Vector3 position, Vector3 targetFinish)
        {
            transform.position = _camera.WorldToScreenPoint(position);
            transform.gameObject.SetActive(true);

            var result = new ReactiveProperty<bool>(false);

            var random = Random.insideUnitSphere;
            var targetEjection = new Vector3(
                transform.position.x + random.x * 100,
                transform.position.y + random.y * 100,
                transform.position.z
            );
            Sequence = DOTween.Sequence();
            
            Sequence
                .Append(
                    transform.GetComponent<RectTransform>()
                        .DOSizeDelta(new Vector2(50, 50), 0.4f)
                        .From(new Vector2(25, 25))
                        .SetEase(Ease.OutQuint)
                        .SetUpdate(true))
                .Join(
                    transform
                        .DOMove(targetEjection, 0.4f)
                        .SetEase(Ease.OutQuint)
                        .SetUpdate(true))
                .Append(DOTween.Sequence().SetDelay(0.1f).SetUpdate(true))
                .Append(
                    transform
                        .DOMove(targetFinish, 0.7f)
                        .SetEase(Ease.InOutCirc)
                        .SetUpdate(true))
                .OnComplete(() =>
                {
                    transform.gameObject.SetActive(false);
                    Sequence.Kill();
                    result.OnNext(true);
                }).SetUpdate(true);
            return result;
        }

        private void OnDestroy()
        {
            if (Sequence.IsActive())
            {
                Sequence.Kill();
                Sequence = null;
            }
        }
    }
}