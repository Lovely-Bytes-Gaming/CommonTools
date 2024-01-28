using LovelyBytes.AssetVariables;
using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    [AddComponentMenu("LovelyBytes/CommonTools/FiniteStateMachine/FsmRunner")]
    public class FsmRunner : MonoBehaviour, IFsmRunner
    {
        public FsmStateMachine StateMachine
        {
            get => _stateMachine;
            set
            {
                if (_stateMachine)
                    _stateMachine.Exit();

                _stateMachine = value;
                Initialize();
            }
        }
        
        [SerializeField, GetSet(nameof(StateMachine))] 
        private FsmStateMachine _stateMachine;
        
        private void OnEnable()
        {
            Initialize();
        }

        private void Update()
        {
            if (_stateMachine)
                _stateMachine.OnUpdate(Time.deltaTime);
        }

        private void OnDisable()
        {
            if(_stateMachine)
                _stateMachine.Exit();
        }

        private void Initialize()
        {
            if (!StateMachine)
                return;
            
            _stateMachine.Enter(runner: this);
        }

        public void Release(FsmStateMachine stateMachine)
        {
            if (_stateMachine == stateMachine)
                _stateMachine = null;
        }
    }
}
