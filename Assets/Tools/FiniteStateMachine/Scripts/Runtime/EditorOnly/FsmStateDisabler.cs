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
        /// The sole purpose of this class is to disable states on playmode exit when they have been enabled
        /// via their custom editor. Cannot be used in production code. 
        /// </summary>
        internal class FsmStateDisabler : MonoBehaviour
        {
            public FsmState State
            {
                get => _state;
                set
                {
                    _state = value;
                    Debug.LogWarning($"Fsm State {_state.name} has been explicitly set active via its custom inspector. " +
                                     $"It will automatically be exited when Play Mode is stopped. Use this feature for debugging only.");
                }
            }

            private FsmState _state;
        
            private void Awake()
            {
                DontDestroyOnLoad(this);
            }

            private void OnDestroy()
            {
                State.Exit();
            }
        }
    }
    #endif
}
