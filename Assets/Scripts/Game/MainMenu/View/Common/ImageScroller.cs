using UnityEngine;
using UnityEngine.UI;

namespace Game.MainMenu.View.Common
{
    [RequireComponent(typeof(RawImage))]
    public class ImageScroller : MonoBehaviour
    {
        private RawImage _image;

        [SerializeField, Range(0, 10)] private float scrollSpeed = 0.1f;

        [SerializeField, Range(-1, 1)] private float xDirection = 1;
        [SerializeField, Range(-1, 1)] private float yDirection = 1;

        private void Awake() => _image = GetComponent<RawImage>();


        private void Update()
            => _image.uvRect =
                new Rect(
                    _image.uvRect.position + new Vector2(-xDirection * scrollSpeed, yDirection * scrollSpeed) *
                    Time.deltaTime, _image.uvRect.size);
    }
}