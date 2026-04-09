using DI;
using Game.GamePlay.Fsm.SkillStates;
using MVVM.FSM;
using R3;
using UnityEngine;

namespace Game.GamePlay.Fsm
{
    public class FsmSkill
    {
        public FsmProxy Fsm;
        
        public ReactiveProperty<Vector2Int> Position = new();

        public FsmSkill(DIContainer container)
        {
            Fsm = new FsmProxy();
            Fsm.AddState(new FsmSkillNone(Fsm, container));
            Fsm.AddState(new FsmSkillBegin(Fsm, container));
            Fsm.AddState(new FsmSkillSetTarget(Fsm, container));
            Fsm.AddState(new FsmSkillShowEffect(Fsm, container));
            Fsm.AddState(new FsmSkillEnd(Fsm, container));
            
            Fsm.SetState<FsmSkillNone>();
        }

        public void SetPosition(Vector2Int position)
        {
            Position.Value = position;
        }
        public void SetPosition(Vector2 position)
        {
            Position.Value = new Vector2Int(
                Mathf.FloorToInt(position.x + 0.5f),
                Mathf.FloorToInt(position.y + 0.5f)
            );
        }
        public string GetConfigId()
        {
            return (string)Fsm.Params;
        }

        public bool IsBegin()
        {
            return Fsm.StateCurrent.CurrentValue.GetType() == typeof(FsmSkillBegin);
        }

        public bool IsNone()
        {
            return Fsm.StateCurrent.CurrentValue.GetType() == typeof(FsmSkillNone);
        }

        public bool IsTarget()
        {
            return Fsm.StateCurrent.CurrentValue.GetType() == typeof(FsmSkillSetTarget);
        }
    }
}