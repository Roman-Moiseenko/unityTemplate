using UnityEngine;

namespace Game.GamePlay.View.Grounds
{
    public class GroundBinder : MonoBehaviour
    {
        [SerializeField] private Material odd;
        [SerializeField] private Material even;
        [SerializeField] private GameObject place;
        public void Bind(GroundViewModel viewModel, int position)
        {
            transform.position = new Vector3(
                viewModel.Position.CurrentValue.x,
                -1,
                viewModel.Position.CurrentValue.y
            );
            if (position == 0) place.GetComponent<MeshRenderer>().material = odd;
            if (position == 1) place.GetComponent<MeshRenderer>().material = even;
            
        }
    }
}