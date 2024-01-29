using System;
using System.Collections.Generic;
using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    #if UNITY_EDITOR
    /// <summary>
    /// The outer class disallows the component to be added in the editor
    /// </summary>
    internal static class FsmStateDisablerEditorGuard
    {
        /// <summary>
        /// The sole purpose of this class is to disable <see cref="FsmState"/> objects on playmode exit when they have been enabled
        /// via their custom editor. Cannot be used in production code. 
        /// </summary>
        internal class FsmStateDisabler : MonoBehaviour
        {
            public static FsmStateDisabler Instance => _instance.Value;
            
            private static readonly Lazy<FsmStateDisabler> _instance = new(() =>
                new GameObject("State Disabler").AddComponent<FsmStateDisabler>());
            
            public void Monitor(FsmState state)
            {
                if (_states.Contains(state))
                    return;
                    
                Debug.LogWarning($"Fsm State {state.name} has been explicitly set active via its custom inspector. " +
                                 $"It will automatically be exited when Play Mode is stopped. " +
                                 $"Note that currently running state machines might override the state again while in play mode.");
                
                _states.Add(state);
            }
            
            private readonly List<FsmState> _states = new();
        
            private void Awake()
            {
                DontDestroyOnLoad(this);
            }

            private void OnDestroy()
            {
                _states.ForEach(state => state.Exit());
            }
        }
    }
    #endif
}
