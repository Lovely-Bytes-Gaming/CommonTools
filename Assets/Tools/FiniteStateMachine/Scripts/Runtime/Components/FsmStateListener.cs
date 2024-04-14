using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    [AddComponentMenu("LovelyBytes/CommonTools/FiniteStateMachine/FsmStateListener")]
    public class FsmStateListener : MonoBehaviour
    {
        [FormerlySerializedAs("_gameState")] [SerializeField] 
        private FsmState _state;

        public UnityEvent OnGameStateEnter;
        public UnityEvent OnGameStateExit;

        private bool _hasStarted = false;

        private void Start()
        {
            _hasStarted = true;
            OnEnable();
        }
        
        private void OnEnable()
        {
            if (!_hasStarted)
                return;
            
            _state.AddListener(
                onEnter: InvokeOnEnter,
                onExit: InvokeOnExit);
        }

        private void OnDisable()
        {
            _state.RemoveListener(
                onEnter: InvokeOnEnter,
                onExit: InvokeOnExit);
        }

        private void InvokeOnEnter()
        {
            OnGameStateEnter?.Invoke();
        }

        private void InvokeOnExit()
        {
            OnGameStateExit?.Invoke();
        }
    }
}
