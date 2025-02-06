#if UNITY_EDITOR

using System.Collections.Generic;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    public partial class FsmStateMachine
    {
        private const string _stateDir = "States";
        private const string _transitionDir = "Transitions";
        private const string _conditionDir = "Conditions";
        private const string _behaviourDir = "Behaviour";
        
        private readonly HashSet<FsmState> _workList = new();

        internal void DoInitializeStatesForEditor()
        {
            InitializeStatesForEditor();
        }
        
        partial void InitializeStatesForEditor()
        {
            AddDescendants(_initialState);
            RemoveDuplicates();
        }
        
        private void RemoveDuplicates()
        {
            _workList.Clear();

            if (_states.Count < 1)
                return;

            for (int i = _states.Count - 1; i > -1; --i)
            {
                FsmState state = _states[i];

                if (_workList.Contains(state))
                    _states.RemoveAt(i);
                else
                    _workList.Add(state);
            }
        }

        private void AddDescendants(FsmState state)
        {
            _workList.Clear();
            AddDescendants(state, 100);
        }        
        
        private void AddDescendants(FsmState state, int maxRecursions)
        {
            if (!state || maxRecursions < 1)
                return;
         
            _states.Add(state);
            _workList.Add(state);
            
            foreach(Transition transition in state.Transitions) 
            {
                if(transition.TargetState && !_workList.Contains(transition.TargetState))
                    AddDescendants(transition.TargetState, maxRecursions - 1);    
            }
        }
    }
}

#endif