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
        private readonly Action<FsmState, FsmState.ViewData> _viewFactory;
        
        public FsmStateView(FsmState state, FsmState.ViewData viewData, 
            Action<FsmState, FsmState.ViewData> viewFactory)
        {
            State = state;
            title = state.name;
            ViewData = viewData;
            viewDataKey = viewData.Guid;
            _viewFactory = viewFactory;
            
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
            evt.menu.AppendAction("Clone Node", _ => CloneView());
        }
        
        private void CloneView()
        {
            FsmState.ViewData viewData = new();
            State.Views.Add(viewData);
            
            _viewFactory?.Invoke(State, viewData);
        }
    }
}