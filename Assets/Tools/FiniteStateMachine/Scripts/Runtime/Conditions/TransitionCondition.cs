
using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    public abstract class TransitionCondition : ScriptableObject
    {
        public abstract bool QueryCondition(float deltaTime);

        public virtual void ResetCondition()
        { }
    }
}