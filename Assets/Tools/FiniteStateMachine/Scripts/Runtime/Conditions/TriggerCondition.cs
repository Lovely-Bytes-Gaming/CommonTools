using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    [CreateAssetMenu(menuName = "LovelyBytes/CommonTools/FiniteStateMachine/TriggerCondition")]
    public class TriggerCondition : TransitionCondition
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