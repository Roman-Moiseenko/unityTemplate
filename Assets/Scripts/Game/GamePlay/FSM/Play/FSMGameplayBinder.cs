using Game.GamePlay.FSM.Play.States;
using UnityEngine;

namespace Game.GamePlay.FSM.Play
{
    public class FSMGameplayBinder : MonoBehaviour
    {
        
        private FSMGameplay FsmGameplay;

        public void Bind(FSMGameplay _fsmGameplay)
        {
            FsmGameplay = _fsmGameplay;
            
        }

        private void Update()
        {
            FsmGameplay?.UpdateState();
        }
    }
    
    
}