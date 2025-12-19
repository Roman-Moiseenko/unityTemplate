using UnityEngine;

namespace Game.GamePlay.View.Grounds
{
    public class BoardWallBinder : MonoBehaviour
    {
        [SerializeField] private Transform leftSide;
        [SerializeField] private Transform rightSide;

        private const float DeltaSide = 0.38f;
        private const float DeltaInAngle = 0.34f;
        private const float DeltaOutAngle = 0.38f;

        public void Bind(BoardWallViewModel viewModel)
        {
            var ratio = viewModel.ConfigId switch
            {
                "Side" => DeltaSide,
                "InAngle" => DeltaInAngle,
                "OutAngle" => DeltaOutAngle,
                _ => 0f
            };

            //gameObject.GetComponent<RectTransform>().
            transform.localEulerAngles = new Vector3(0, viewModel.Rotation, 0);
            transform.localPosition = new Vector3(
                transform.localPosition.x + viewModel.DeltaX * ratio,
                transform.localPosition.y,
                transform.localPosition.z + viewModel.DeltaY * ratio
            );

            if (viewModel.ConfigId == "Side")
            {
                leftSide?.gameObject.SetActive(viewModel.ShowLeftSide);
                rightSide?.gameObject.SetActive(viewModel.ShowRightSide);
            }
        }
    }
}