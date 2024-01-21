using LovelyBytes.AssetVariables;
using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    [AddComponentMenu("LovelyBytes/CommonTools/FiniteStateMachine/FsmRunner")]
    public class FsmRunner : MonoBehaviour
    {
        public FsmStateMachine StateMachine
        {
            get => _stateMachine;
            set
            {
                if (_stateMachine)
                    _stateMachine.Exit();

                _stateMachine = value;
                _stateMachine.Enter();
            }
        }
        
        [SerializeField, GetSet(nameof(StateMachine))] 
        private FsmStateMachine _stateMachine;
        
        private void OnEnable()
        {
            if(_stateMachine)
                _stateMachine.Enter();
        }

        private void Update()
        {
            _stateMachine.OnUpdate(Time.deltaTime);
        }

        private void OnDisable()
        {
            if(_stateMachine)
                _stateMachine.Exit();
        }
    }
}
