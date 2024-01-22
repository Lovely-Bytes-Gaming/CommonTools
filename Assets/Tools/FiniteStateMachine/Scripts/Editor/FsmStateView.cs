using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    internal class FsmStateView : Node
    {
        public Port Input, Output;
        public readonly FsmState State;
        public readonly FsmState.ViewData ViewData;
        public readonly FsmStateMachine StateMachine;
        
        private readonly Action<FsmState, FsmState.ViewData> _viewFactory;
        
        public FsmStateView(FsmState state, FsmState.ViewData viewData, 
            Action<FsmState, FsmState.ViewData> viewFactory, FsmStateMachine stateMachine)
        {
            State = state;
            title = state.name;
            ViewData = viewData;
            viewDataKey = viewData.Guid;
            _viewFactory = viewFactory;
            StateMachine = stateMachine;
            
            style.left = viewData.CanvasPosition.x;
            style.top = viewData.CanvasPosition.y;

            CreateInputPorts();
            CreateOutputPorts();
        }

        private void CreateOutputPorts()
        {
            Output = InstantiatePort(Orientation.Horizontal, Direction.Output, 
                Port.Capacity.Multi, typeof(FsmState));
            
            if(Output == null)
                return;

            Output.portName = string.Empty;
            outputContainer.Add(Output);
        }

        private void CreateInputPorts()
        {
            Input = InstantiatePort(Orientation.Horizontal, Direction.Input, 
                Port.Capacity.Multi, typeof(FsmState));
            
            if(Input == null)
                return;
            
            Input.portName = string.Empty;
            inputContainer.Add(Input);
        }

        public sealed override string title
        {
            get => base.title;
            set => base.title = value;
        }

        public override void SetPosition(Rect newPosition)
        {
            base.SetPosition(newPosition);
            Vector2 canvasPosition = new(newPosition.xMin, newPosition.yMin);
            
            ViewData.CanvasPosition = canvasPosition;
            EditorUtility.SetDirty(State);
        }
        
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("State/Clone Node", _ => CloneView());
            
            AppendBehaviourActions(evt);
            
            if(Application.isPlaying)
                AppendPlaymodeActions(evt);
        }

        private void AppendPlaymodeActions(ContextualMenuPopulateEvent evt)
        {
            if (State.IsActive)
                evt.menu.AppendAction("State/Exit", _ => StateMachine.Exit());
            else
                evt.menu.AppendAction("State/Enter", _ => EnterState(State));
        }

        private void EnterState(FsmState state)
        {
            CreateRunner();
            StateMachine.JumpTo(state);
        }
        
        private void AppendBehaviourActions(ContextualMenuPopulateEvent evt)
        {
            TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom<FsmBehaviour>();
            
            foreach(Type type in types)
                AppendCreateBehaviourAction(evt, type);
        }
        
        private void AppendCreateBehaviourAction(ContextualMenuPopulateEvent evt, Type type)
        {
            if (type.IsGenericType)
                return;
            
            evt.menu.AppendAction($"State/Add Behaviour/{type.Name}", _ =>
            {
                if (!State || !StateMachine)
                    return;
                
                FsmFactory.CreateBehaviour(StateMachine, State, type);
            });
        }
        
        private void CloneView()
        {
            FsmState.ViewData viewData = new();
            State.Views.Add(viewData);
            
            _viewFactory?.Invoke(State, viewData);
        }
        
        private void CreateRunner()
        {
            if (FsmGlobalContext.Instance.HasRunner(StateMachine)) 
                return;
            
            FsmRunner runner = new GameObject($"{StateMachine.name}_Runner")
                .AddComponent<FsmRunner>();

            runner.StateMachine = StateMachine;
        }
    }
}