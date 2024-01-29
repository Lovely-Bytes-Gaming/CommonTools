using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    /// <summary>
    /// Scriptable objects inheriting from this class can be attached to <see cref="FsmState"/> objects.
    /// </summary>
    public abstract class FsmBehaviour : ScriptableObject
    {
        /// <summary>
        /// Called when the <see cref="FsmState"/> this behaviour is attached to is entered.
        /// </summary>
        public virtual void OnStart() {}
        /// <summary>
        /// Called when the <see cref="FsmState"/> this behaviour is attached to is exited.
        /// </summary>
        public virtual void OnStop() {}
        /// <summary>
        /// Called every frame during which the <see cref="FsmState"/> this behaviour is attached to is active.
        /// </summary>
        public abstract void OnUpdate(float deltaTime);
    }
}