using LovelyBytes.AssetVariables;
using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    [CreateAssetMenu(menuName = "LovelyBytes/CommonTools/FiniteStateMachine/BoolCondition")]
    public class BoolCondition : FsmCondition
    {
        [SerializeField] 
        private BoolVariable _boolVariable;

        public bool ReferenceValue = true;   
        
        public override bool QueryCondition(float deltaTime)
        {
            return _boolVariable.Value == ReferenceValue;
        }
    }
}
