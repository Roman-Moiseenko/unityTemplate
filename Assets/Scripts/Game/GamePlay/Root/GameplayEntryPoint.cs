using UnityEngine;

namespace Game.GamePlay.Root
{
    public class GameplayEntryPoint : MonoBehaviour
    {
        [SerializeField] private GameObject _sceneRootBinder;

        public void Run()
        {
            Debug.Log("Game play entry point");
        }
    }
}