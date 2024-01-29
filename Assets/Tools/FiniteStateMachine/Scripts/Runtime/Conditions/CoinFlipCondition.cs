using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    [CreateAssetMenu(menuName = "LovelyBytes/CommonTools/FiniteStateMachine/CoinFlipCondition")]
    public class CoinFlipCondition : TransitionCondition
    {
        [SerializeField, Range(0f, 1f)] 
        private float _chanceOfSuccess = 0.5f;

        public float ChanceOfSuccess
        {
            get => _chanceOfSuccess;
            set => _chanceOfSuccess = Mathf.Clamp01(value);
        }
        
        public override bool QueryCondition(float deltaTime)
        {
            return Random.value < _chanceOfSuccess;
        }
    }
}