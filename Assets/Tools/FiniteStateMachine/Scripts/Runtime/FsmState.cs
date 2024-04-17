using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    /// <summary>
    /// Represents one of multiple states an <see cref="FsmStateMachine"/> can be in.
    /// </summary>
    [CreateAssetMenu(menuName = "LovelyBytes/CommonTools/FiniteStateMachine/FsmState")]
    public partial class FsmState : ScriptableObject, IFsmRunner
    {
        public bool IsActive { get; private set; }
        
        [field: SerializeField] 
        public List<FsmTransition> Transitions { get; private set; } = new();

        [field: SerializeField] 
        public List<FsmBehaviour> Behaviours { get; private set; } = new();

        public FsmStateMachine SubStateMachine
        {
            get => _subStateMachine;
            set
            {
                FsmStateMachine oldFsm = _subStateMachine;

                _subStateMachine = value;

                if (!IsActive) 
                    return;
                
                if (oldFsm)
                    oldFsm.Exit();
                
                if(_subStateMachine)
                    _subStateMachine.Enter(runner: this);
            }
        }

        [FormerlySerializedAs("SubStateMachine")] [SerializeField, HideInInspector]
        private FsmStateMachine _subStateMachine;
        
        [SerializeField]
        private UnityEvent _onEnter;
        
        [SerializeField]
        private UnityEvent _onExit;
        
        /// <summary>
        /// Will automatically invoke 'onEnter' when this state is already active.
        /// </summary>
        public void AddListener(UnityAction onEnter, UnityAction onExit = null)
        {
            if (IsActive)
                onEnter?.Invoke();
            else
                onExit?.Invoke();
            
            if (onEnter != null)
                _onEnter.AddListener(onEnter);
            
            if (onExit != null)
                _onExit.AddListener(onExit);
        }

        public void RemoveListener(UnityAction onEnter, UnityAction onExit = null)
        {
            if (onEnter != null)
                _onEnter.RemoveListener(onEnter);

            if (onExit != null)
                _onExit.RemoveListener(onExit);
        }
        
        public void Release(FsmStateMachine stateMachine)
        {
            if (_subStateMachine == stateMachine)
                _subStateMachine = null;
        }
        
        internal void Enter()
        {
            IsActive = true;
            
            ResetTransitions();
            StartBehaviours();
            
            _onEnter?.Invoke();

            if (_subStateMachine)
                _subStateMachine.Enter(runner: this);
        }
        
        internal void Exit()
        {
            StopBehaviours();
            
            if (_subStateMachine)
                _subStateMachine.Exit();
            
            IsActive = false;
            _onExit?.Invoke();
        }

        internal bool OnUpdate(float deltaTime, out FsmTransition firedFsmTransition)
        {
            UpdateBehaviours(deltaTime);
            
            if (_subStateMachine)
                _subStateMachine.OnUpdate(deltaTime);
            
            return TryGetValidTransition(deltaTime, out firedFsmTransition);
        }

        private void ResetTransitions()
        {
            foreach(FsmTransition transition in Transitions)
                transition.ResetConditions();
        }

        private void StartBehaviours()
        {
            foreach (FsmBehaviour behaviour in Behaviours)
                behaviour.OnStart();
        }
        
        private void StopBehaviours()
        {
            foreach (FsmBehaviour behaviour in Behaviours)
                behaviour.OnStop();
        }

        private void UpdateBehaviours(float deltaTime)
        {
            foreach(FsmBehaviour behaviour in Behaviours)
                behaviour.OnUpdate(deltaTime);
        }
        
        private bool TryGetValidTransition(float deltaTime, out FsmTransition fsmTransition)
        {
            fsmTransition = null;
            
            foreach (FsmTransition t in Transitions)
            {
                if (!t.QueryConditions(deltaTime))
                    continue;

                fsmTransition = t;
                return true;
            }
            return false;
        }
    }
}