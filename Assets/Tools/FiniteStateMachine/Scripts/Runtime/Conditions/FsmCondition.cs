
using LovelyBytes.AssetVariables;
using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    /// <summary>
    /// Can be attached to a <see cref="FsmTransition"/> to control under which condition it can be fired.
    /// </summary>
    public abstract class FsmCondition : ScriptableObject
    {
        /// <summary>
        /// When this condition is set and is satisfied, it will take priority over the containing condition.
        /// </summary>
        public FsmCondition OverrideCondition
        {
            get => _overrideCondition;
            set
            {
                // don't allow cyclic override conditions!
                if (value.OverrideCondition == this)
                    return;

                _overrideCondition = value;
            }
        }
        
        [SerializeField, GetSet(nameof(OverrideCondition))]
        private FsmCondition _overrideCondition;

        public bool IsSatisfied => (OverrideCondition && OverrideCondition.IsSatisfied) ||
                                   GetIsSatisfied();
        
        /// <summary>
        /// Called once before this Condition is queried for the first time.
        /// </summary>
        public virtual void ResetCondition()
        { }
        
        /// <summary>
        /// Called each frame when the <see cref="FsmStateMachine"/> that this condition belongs to is updated.
        /// </summary>
        /// <param name="deltaTime">The time since the last state machine update.</param>
        public virtual void UpdateCondition(float deltaTime) { }

        /// <summary>
        /// Override this method to implement the conditional logic that determines whether this condition is satisfied.
        /// </summary>
        /// <returns>Whether the condition is satisfied.</returns>
        protected abstract bool GetIsSatisfied();
    }
}