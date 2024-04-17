
using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    /// <summary>
    /// Can be attached to a <see cref="FsmTransition"/> to control under which condition it can be fired.
    /// </summary>
    public abstract class FsmCondition : ScriptableObject
    {
        /// <summary>
        /// Called each frame when the <see cref="FsmStateMachine"/> this condition belongs to is updated.
        /// </summary>
        /// <param name="deltaTime">The time since the last state machine update.</param>
        /// <returns>Whether the condition is satisfied.</returns>
        public abstract bool QueryCondition(float deltaTime);

        /// <summary>
        /// Called once before this Condition is queried for the first time.
        /// </summary>
        public virtual void ResetCondition()
        { }
    }
}