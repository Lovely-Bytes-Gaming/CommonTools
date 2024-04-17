using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    /// <summary>
    /// Waits for set time before evaluating to true.
    /// </summary>
    [CreateAssetMenu(menuName = "LovelyBytes/CommonTools/FiniteStateMachine/TimerCondition")]
    public class TimerCondition : FsmCondition
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