using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    [CreateAssetMenu(menuName = "LovelyBytes/CommonTools/FiniteStateMachine/TimerCondition")]
    public class TimerCondition : TransitionCondition
    {
        public float Duration = 1f;
        private float _time;

        public override void ResetCondition()
        {
            ResetTimer();
        }
        
        public override bool QueryCondition(float deltaTime)
        {
            _time += deltaTime;
            return _time > Duration;
        }

        public void ResetTimer()
        {
            _time = 0f;
        }
    }
}