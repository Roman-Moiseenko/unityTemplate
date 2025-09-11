using System;
using System.Collections;
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
        public Animator animator;

        private ImageManagerBinder _imageManager;
        private Vector3 _target;
        private IDisposable _disposable;
        
        public void Bind()
        {
            var d = Disposable.CreateBuilder();
            RewardState = new ReactiveProperty<CurrencyState>();
            _imageManager = GameObject.Find(AppConstants.IMAGE_MANAGER).GetComponent<ImageManagerBinder>();
            _target = endPoint.transform.position;
            animator = gameObject.GetComponent<Animator>();
            animator.enabled = false;
            endPoint.gameObject.SetActive(false);
            moving.gameObject.SetActive(false);
            
            RewardState.Skip(1).Subscribe(state =>
            {
                if (state == CurrencyState.Animation) animator.enabled = true;
                if (state == CurrencyState.Ejection)
                {
                    animator.enabled = false;
                    var random = Random.insideUnitSphere;
                    var target = new Vector3(
                        moving.transform.position.x + random.x * 160,
                        moving.transform.position.y + random.y * 160,
                        moving.transform.position.z
                    );
                    StartCoroutine(Ejection(target));
                }

                if (state == CurrencyState.Delay) StartCoroutine(DelayMoving());
                if (state == CurrencyState.Moving)
                {
                    endPoint.transform.gameObject.SetActive(true);
                    StartCoroutine(Moving(_target));
                }
                if (state == CurrencyState.Rest)
                {
                    moving.transform.gameObject.SetActive(false);
                    endPoint.transform.gameObject.SetActive(false);
                }
                
            }).AddTo(ref d);
            _disposable = d.Build();
            
            //Движение
        }
 
        public void StartPopup(RewardEntityType rewardType, string configId, Vector3 position)
        {
            if (rewardType == RewardEntityType.TowerCard)
            {
                imageCard.sprite = _imageManager.GetTowerCard(configId, 1);
                imageBack.sprite = _imageManager.GetEpicLevel(TypeEpicCard.Normal);
              //  imageCard.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
            }

            if (rewardType == RewardEntityType.TowerPlan)
            {
                imageCard.sprite = _imageManager.GetTowerPlan(configId);
                imageBack.sprite = _imageManager.GetTowerPlan("Background");
               // imageCard.GetComponent<RectTransform>().sizeDelta = new Vector2(80, 80);
            }
            moving.transform.position = position;
            moving.transform.gameObject.SetActive(true);
            RewardState.Value = CurrencyState.Animation;
        }
        
       
        private IEnumerator DelayMoving()
        {
            yield return new WaitForSeconds(0.5f);
            RewardState.Value = CurrencyState.Moving;
        }

        private IEnumerator Moving(Vector3 target)
        {
            float timeElapsed = 0;
            var duration = 0.7f;
            var startPosition = moving.transform.position;
            var fading = false; 
            
            while (timeElapsed < duration)
            {
                if (timeElapsed > duration / 0.8) fading = true;
                var delta = !fading ? Time.deltaTime : Time.deltaTime / 2;
                moving.transform.position = Vector3.Lerp(startPosition, target, timeElapsed / duration);
                timeElapsed += delta;
                yield return null;
            }
            moving.transform.position = target;
            RewardState.Value = CurrencyState.Rest;
        }
        
        private IEnumerator Ejection(Vector3 target)
        {
            float timeElapsed = 0;
            float duration = 0.8f;
            
            while (timeElapsed < duration)
            {
                moving.transform.position = Vector3.Lerp(moving.transform.position, target, timeElapsed / duration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            moving.transform.position = target;
            RewardState.Value = CurrencyState.Delay;
        }
        private void OnDestroy()
        {
            _disposable.Dispose();
        }
    }
}