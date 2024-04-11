using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    [CreateAssetMenu(menuName = "LovelyBytes/CommonTools/FiniteStateMachine/SubStateMachineFinished")]
    public class SubStateMachineFinished : TransitionCondition
    {
        [SerializeField] private FsmState _state;
        
        public override bool QueryCondition(float deltaTime)
        {
            FsmStateMachine subFsm = _state.SubStateMachine;
            
            if (!subFsm)
                return true;

            return subFsm.CurrentState == subFsm.ExitState;
        }
    }
}