using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    [CreateAssetMenu(menuName = "LovelyBytes/CommonTools/FiniteStateMachine/FsmState")]
    public partial class FsmState : ScriptableObject, IFsmRunner
    {
        public bool IsActive { get; private set; }
        
        [field: SerializeField] 
        public List<Transition> Transitions { get; private set; } = new();

        [field: SerializeField] 
        public List<FsmBehaviour> Behaviours { get; private set; } = new();

        public FsmStateMachine StateMachine
        {
            get => _subStateMachine;
            set
            {
                FsmStateMachine oldFsm = _subStateMachine;

                _subStateMachine = value;
                FsmGlobalContext.Instance.RegisterRunner(this);

                if (!IsActive) 
                    return;
                
                if (oldFsm)
                    oldFsm.Exit();
                
                if(_subStateMachine)
                    _subStateMachine.Enter();
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
        
        internal void Enter()
        {
            IsActive = true;
            
            ResetTransitions();
            StartBehaviours();
            
            _onEnter?.Invoke();

            if (_subStateMachine)
            {
                FsmGlobalContext.Instance.RegisterRunner(this);
                _subStateMachine.Enter();
            }
        }
        
        internal void Exit()
        {
            StopBehaviours();
            
            if (_subStateMachine)
                _subStateMachine.Exit();
            
            IsActive = false;
            _onExit?.Invoke();
        }

        internal bool OnUpdate(float deltaTime, out Transition firedTransition)
        {
            UpdateBehaviours(deltaTime);
            
            if (_subStateMachine)
                _subStateMachine.OnUpdate(deltaTime);
            
            return TryGetValidTransition(deltaTime, out firedTransition);
        }

        private void ResetTransitions()
        {
            foreach(Transition transition in Transitions)
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
        
        private bool TryGetValidTransition(float deltaTime, out Transition transition)
        {
            transition = null;
            
            foreach (Transition t in Transitions)
            {
                if (!t.QueryConditions(deltaTime))
                    continue;

                transition = t;
                return true;
            }
            return false;
        }
    }
}