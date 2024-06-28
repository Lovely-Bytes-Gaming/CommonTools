using LovelyBytes.AssetVariables;
using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    [CreateAssetMenu(menuName = "LovelyBytes/CommonTools/FiniteStateMachine/BoolCondition")]
    public class BoolCondition : FsmCondition
    {
        public bool ReferenceValue = true;   
        
        [SerializeField] 
        private BoolVariable _boolVariable;
        
        protected override bool GetIsSatisfied()
        {
            return _boolVariable.Value == ReferenceValue;
        }
    }
}
