using System.Collections.Generic;
using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    /// <summary>
    /// <see cref="ScriptableObject"/> representation of a Finite State Machine:
    /// https://en.wikipedia.org/wiki/Finite-state_machine
    /// For editing, use the custom editor window at:
    /// Window -> Lovely Bytes -> State Machine Editor
    /// </summary>
    [CreateAssetMenu(menuName = "LovelyBytes/CommonTools/FiniteStateMachine/FsmStateMachine")]
    public partial class FsmStateMachine : ScriptableObject
    {
        public IReadOnlyList<FsmState> States => _statesRO ??= _states.AsReadOnly();
        public bool IsRunning => _runner != null;
        
        public FsmState InitialState
        {
            get => _initialState;
            set
            {
                if (_states.Contains(value))
                    _initialState = value;
            }
        }
        
        [SerializeField] 
        private List<FsmState> _states = new();
        private IReadOnlyList<FsmState> _statesRO;
        
        [SerializeField] 
        private FsmState _initialState;

        private FsmState _current;
        private IFsmRunner _runner;

        partial void InitializeStatesForEditor();
        
        public void Enter(IFsmRunner runner)
        {
            _runner?.Release(this);
            _runner = runner;
            
            InitializeStatesForEditor();
            ResetStates();
            
            _current = _initialState;
            _current.Enter();
        }

        public void Exit()
        {
            ResetStates();
            _current = null;
            _runner = null;
        }
        
        public void OnUpdate(float deltaTime)
        {
            if (_current && _current.OnUpdate(deltaTime, out Transition transition))
                SetState(transition.TargetState);
        }

        public void AddState(FsmState state)
        {
            if (!_initialState)
                _initialState = state;
            
            if (!_states.Contains(state))
                _states.Add(state);
        }

        public void RemoveState(FsmState state)
        {
            if (_initialState == state)
                _initialState = null;

            _states.Remove(state);
        }

        public void JumpTo(FsmState state)
        {
            if (_states.Contains(state) && (_current != state || !_current.IsActive))
                SetState(state);
        }
        
        private void SetState(FsmState state) 
        {
            if(_current)
                _current.Exit();
            
            _current = state;
            _current.Enter();
        }
        
        private void ResetStates()
        {
            foreach (FsmState state in _states)
                state.Exit();
        }
    }
}
