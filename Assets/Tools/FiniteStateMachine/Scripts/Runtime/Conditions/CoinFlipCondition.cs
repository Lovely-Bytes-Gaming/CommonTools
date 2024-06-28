using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    /// <summary>
    /// Condition that performs a coin flip every time it is queried, returning whether the flip was won.
    /// </summary>
    [CreateAssetMenu(menuName = "LovelyBytes/CommonTools/FiniteStateMachine/CoinFlipCondition")]
    public class CoinFlipCondition : FsmCondition
    {
        [SerializeField, Range(0f, 1f)] 
        private float _chanceOfSuccess = 0.5f;

        /// <summary>
        /// Controls the probability of winning the coin flip. For a value of 1, the coin flip is always won,
        /// for 0 always lost.
        /// </summary>
        public float ChanceOfSuccess
        {
            get => _chanceOfSuccess;
            set => _chanceOfSuccess = Mathf.Clamp01(value);
        }
        
        protected override bool GetIsSatisfied()
        {
            return Random.value < _chanceOfSuccess;
        }
    }
}