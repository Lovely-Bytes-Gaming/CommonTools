using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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
        public IReadOnlyList<FsmState> States => _states.AsReadOnly();
        public bool IsRunning => _runner != null;
        
        public FsmState EntryState
        {
            get => _entryState;
            set
            {
                if (_states.Contains(value))
                    _entryState = value;
            }
        }

        public FsmState ExitState
        {
            get => _exitState;
            set
            {
                if (_states.Contains(value))
                    _exitState = value;
            }
        }
        
        public FsmState CurrentState { get; private set; }
        
        [SerializeField] 
        private List<FsmState> _states = new();
        
        [FormerlySerializedAs("_initialState")] [SerializeField] 
        private FsmState _entryState;

        [SerializeField] 
        private FsmState _exitState;

        private IFsmRunner _runner;

        partial void InitializeStatesForEditor();
        
        public void Enter(IFsmRunner runner)
        {
            _runner?.Release(this);
            _runner = runner;
            
            InitializeStatesForEditor();
            ResetStates();
            
            CurrentState = _entryState;
            CurrentState.Enter();
        }

        public void Exit()
        {
            ResetStates();
            CurrentState = null;
            _runner = null;
        }
        
        public void OnUpdate(float deltaTime)
        {
            if (CurrentState && CurrentState.OnUpdate(deltaTime, out FsmTransition transition))
                SetState(transition.TargetState);
        }

        public void AddState(FsmState state)
        {
            if (!_states.Contains(state))
                _states.Add(state);
        }

        public void RemoveState(FsmState state)
        {
            // cannot remove entry or exit state
            if (state == _entryState || state == _exitState)
                return;

            _states.Remove(state);
        }

        public void JumpTo(FsmState state)
        {
            if (_states.Contains(state) && (CurrentState != state || !CurrentState.IsActive))
                SetState(state);
        }
        
        private void SetState(FsmState state) 
        {
            if(CurrentState)
                CurrentState.Exit();
            
            CurrentState = state;
            CurrentState.Enter();
        }
        
        private void ResetStates()
        {
            foreach (FsmState state in _states)
                state.Exit();
        }
    }
}
