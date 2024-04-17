using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    /// <summary>
    /// Waits for a random time between MinTime and MaxTime before evaluating to true. 
    /// </summary>
    [CreateAssetMenu(menuName = "LovelyBytes/CommonTools/FiniteStateMachine/RandomTimerCondition")]
    public class RandomTimerCondition : FsmCondition
    {
        public float MinTime = 1f;
        public float MaxTime = 2f;

        private float _currentDuration;
        private float _time;


        public override bool QueryCondition(float deltaTime)
        {
            _time += deltaTime;
            return _time > _currentDuration;
        }

        public override void ResetCondition()
        {
            _time = 0f;
            _currentDuration = Random.Range(MinTime, MaxTime);
        }
    }
}