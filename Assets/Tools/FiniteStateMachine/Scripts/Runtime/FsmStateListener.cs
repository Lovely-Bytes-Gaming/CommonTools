using UnityEngine;
using UnityEngine.Events;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    [AddComponentMenu("LovelyBytes/CommonTools/FiniteStateMachine/FsmStateListener")]
    public class FsmStateListener : MonoBehaviour
    {
        [SerializeField] 
        private FsmState _gameState;

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
            
            _gameState.AddListener(
                onEnter: InvokeOnEnter,
                onExit: InvokeOnExit);
        }

        private void OnDisable()
        {
            _gameState.RemoveListener(
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
