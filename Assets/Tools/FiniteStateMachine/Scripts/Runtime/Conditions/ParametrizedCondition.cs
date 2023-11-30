using System;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    /// <summary>
    /// Allows to inject implementation logic via constructor parameters.
    /// Can only be instantiated via script
    /// </summary>
    public class ParametrizedCondition : TransitionCondition
    {
        private Func<float, bool> _condition;
        private Action _onFire;
        
        public void SetCallbacks(Func<float, bool> condition, Action onFire = null)
        {
            _condition = condition;
            _onFire = onFire;
        }

        public override bool QueryCondition(float deltaTime)
            => _condition == null || _condition.Invoke(deltaTime);

        public override void ResetCondition()
            => _onFire?.Invoke();
    }
}