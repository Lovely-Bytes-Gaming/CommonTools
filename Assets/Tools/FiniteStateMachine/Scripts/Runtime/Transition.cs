using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    [CreateAssetMenu(menuName = "LovelyBytes/CommonTools/FiniteStateMachine/Transition")]
    public partial class Transition : ScriptableObject
    {
        [SerializeField] 
        public FsmState TargetState;

        [field: SerializeField] 
        public List<TransitionCondition> Conditions { get; private set; } = new();

        public bool QueryConditions(float deltaTime)
        {
            foreach (TransitionCondition condition in Conditions)
            {
                if (condition.QueryCondition(deltaTime))
                    return true;
            }
            return false;
        }

        public void ResetConditions()
        {
            if (Conditions == null)
                return;
            
            foreach(TransitionCondition condition in Conditions)
                condition.ResetCondition();
        }
    }
}