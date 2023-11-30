using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    public abstract class FsmBehaviour : ScriptableObject
    {
        public virtual void OnStart() {}
        public virtual void OnStop() {}
        public abstract void OnUpdate(float deltaTime);
    }
}