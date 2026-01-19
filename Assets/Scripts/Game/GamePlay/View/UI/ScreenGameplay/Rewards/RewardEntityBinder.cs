using System;
using System.Collections;
using DG.Tweening;
using Game.Common;
using Game.GameRoot.ImageManager;
using Game.State.Inventory;
using Game.State.Maps.Rewards;
using R3;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Game.GamePlay.View.UI.ScreenGameplay.Rewards
{
    public class RewardEntityBinder : MonoBehaviour
    {
        [SerializeField] private Image imageCard;
        [SerializeField] private Image imageBack;
        [SerializeField] private Transform endPoint;
        [SerializeField] private Transform moving;
        
        public ReactiveProperty<CurrencyState> RewardState;

        private ImageManagerBinder _imageManager;
        private Vector3 _targetFinish;
        private Sequence Sequence { get; set; }

        public void Bind()
        {
            RewardState = new ReactiveProperty<CurrencyState>();
            _imageManager = GameObject.Find(AppConstants.IMAGE_MANAGER).GetComponent<ImageManagerBinder>();
            _targetFinish = endPoint.transform.position;

            endPoint.gameObject.SetActive(false);
            moving.gameObject.SetActive(false);
        }

        public void StartPopup(InventoryType rewardType, string configId, Vector3 position)
        {
            if (rewardType == InventoryType.TowerCard)
            {
                imageCard.sprite = _imageManager.GetTowerCard(configId, 1);
                imageBack.sprite = _imageManager.GetEpicLevel(TypeEpicCard.Normal);
            }

            if (rewardType == InventoryType.TowerPlan)
            {
                imageCard.sprite = _imageManager.GetTowerPlan(configId);
                imageBack.sprite = _imageManager.GetTowerPlan("Background");
            }

            moving.transform.position = position;
            moving.transform.gameObject.SetActive(true);

            var random = Random.insideUnitSphere;
            var targetEjection = new Vector3(
                moving.transform.position.x + random.x * 160,
                moving.transform.position.y + random.y * 160,
                moving.transform.position.z
            );
            endPoint.gameObject.SetActive(true);
            Sequence = DOTween.Sequence();
            Sequence
                .Append(
                    moving.transform
                        .DOScale(new Vector3(1f, 1f, 1f), 0.5f)
                        .From(new Vector3(0.5f, 0.5f, 1f))
                        .SetEase(Ease.OutQuint)
                        .SetUpdate(true))
                .Join(
                    moving.transform
                        .DOMove(targetEjection, 0.5f)
                        .SetEase(Ease.OutQuint)
                        .SetUpdate(true))
                .Append(DOTween.Sequence().SetDelay(0.3f).SetUpdate(true))
                .Append(
                    moving.transform
                        .DOMove(_targetFinish, 0.7f)
                        .SetEase(Ease.InOutCirc)
                        .SetUpdate(true))
                .OnComplete(() =>
                {
                    moving.transform.gameObject.SetActive(false);
                    endPoint.transform.gameObject.SetActive(false);
                    Sequence.Kill();
                }).SetUpdate(true);

            RewardState.Value = CurrencyState.Animation;
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