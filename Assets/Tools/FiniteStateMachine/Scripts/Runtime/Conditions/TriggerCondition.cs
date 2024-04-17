using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    /// <summary>
    /// Condition that can be referenced and fired from anywhere.
    /// Returns whether the trigger has already been fired.
    /// </summary>
    [CreateAssetMenu(menuName = "LovelyBytes/CommonTools/FiniteStateMachine/TriggerCondition")]
    public class TriggerCondition : FsmCondition
    {
        private bool _canFire;

        public void Fire()
        {
            _canFire = true;
        }

        public override bool QueryCondition(float deltaTime)
        {
            return _canFire;
        }

        public override void ResetCondition()
        {
            _canFire = false;
        }
    }
}