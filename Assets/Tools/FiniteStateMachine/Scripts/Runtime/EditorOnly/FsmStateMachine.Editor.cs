#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    public partial class FsmStateMachine
    {
        private readonly HashSet<FsmState> _workList = new();

        internal void RecalculateNames()
        {
            foreach (FsmState state in _states)
            {
                foreach (FsmBehaviour behaviour in state.Behaviours)
                    behaviour.name = GetChildName(state, behaviour);
                
                foreach (Transition transition in state.Transitions)
                {
                    transition.name = GetTransitionName(state, transition.TargetState);
                    
                    foreach (TransitionCondition condition in transition.Conditions)
                        condition.name = GetChildName(transition, condition);
                }
            }
            AssetDatabase.SaveAssets();
        }
        
        internal void DoInitializeStatesForEditor()
        {
            InitializeStatesForEditor();
        }
        
        partial void InitializeStatesForEditor()
        {
            AddDescendants(_entryState, 100);
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

        private void AddDescendants(FsmState state, int maxRecursions)
        {
            if (!state || maxRecursions < 1)
                return;
         
            _workList.Clear();
            
            _states.Add(state);
            _workList.Add(state);
            
            foreach(Transition transition in state.Transitions) 
            {
                if(transition.TargetState && !_workList.Contains(transition.TargetState))
                    AddDescendants(transition.TargetState, maxRecursions - 1);    
            }
        }
        
        private static string GetTransitionName(Object from, Object to)
            => $"{from.name} -> {to.name}";

        private static string GetChildName(Object parent, Object child)
            => $"{parent.name}: {child.GetType().Name}";
    }
}

#endif