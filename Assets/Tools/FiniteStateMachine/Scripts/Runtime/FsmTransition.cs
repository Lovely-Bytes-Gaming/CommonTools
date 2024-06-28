using System.Collections.Generic;
using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    [CreateAssetMenu(menuName = "LovelyBytes/CommonTools/FiniteStateMachine/Transition")]
    public partial class FsmTransition : ScriptableObject
    {
        [SerializeField] 
        public FsmState TargetState;

        [field: SerializeField] 
        public List<FsmCondition> Conditions { get; private set; } = new();

        public bool QueryConditions(float deltaTime)
        {
            if (Conditions.Count < 1)
                return true;
            
            foreach (FsmCondition condition in Conditions)
            {
                condition.UpdateCondition(deltaTime);
                
                if (condition.IsSatisfied)
                    return true;
            }
            return false;
        }

        public void ResetConditions()
        {
            if (Conditions == null)
                return;
            
            foreach(FsmCondition condition in Conditions)
                condition.ResetCondition();
        }
    }
}