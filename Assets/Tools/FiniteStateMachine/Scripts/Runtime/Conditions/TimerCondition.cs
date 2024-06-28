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

        public override void UpdateCondition(float deltaTime)
        {
            _time += deltaTime;
        }
        
        public void ResetTimer()
        {
            _time = 0f;
        }
        
        protected override bool GetIsSatisfied()
        {
            return _time > Duration;
        }
    }
}