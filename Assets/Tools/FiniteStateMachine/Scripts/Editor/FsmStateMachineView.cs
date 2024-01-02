using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace LovelyBytes.CommonTools.FiniteStateMachine 
{
    public class FsmStateMachineView : GraphView 
    {
        public new class UxmlFactory : UxmlFactory<FsmStateMachineView, UxmlTraits> { }

        private FsmStateMachine _stateMachine;
        
        public FsmStateMachineView()
        {
            Insert(0, new GridBackground());
            
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            string rootDirectory = UIBuilderUtils.GetParentDirectory(nameof(FsmStateMachineView));
            
            StyleSheet styleSheet = AssetDatabase
                .LoadAssetAtPath<StyleSheet>($"{rootDirectory}/FsmStateMachineEditor.uss");
            
            styleSheets.Add(styleSheet);
        }

        public void PopulateView(FsmStateMachine stateMachine)
        {
            _stateMachine = stateMachine;

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;

            CreateViews(_stateMachine.States);

            foreach (FsmState state in _stateMachine.States)
                CreateEdges(state);
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            graphViewChange.elementsToRemove?.ForEach(element =>
            {
                switch (element)
                {
                    case FsmStateView stateView:
                    {
                        FsmFactory.DeleteViewData(_stateMachine, stateView.State, stateView.ViewData);
                        break;
                    }
                    case Edge edge when
                        edge.output.node is FsmStateView from &&
                        edge.input.node is FsmStateView to:
                        
                        FsmFactory.DeleteTransition(from.State, to.State);
                        break;
                }
            });

            graphViewChange.edgesToCreate?.ForEach(edge =>
            {
                if (edge.output.node is FsmStateView from && edge.input.node is FsmStateView to) 
                    FsmFactory.CreateTransition(_stateMachine, from, to);
            });
            
            return graphViewChange;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (!FsmStateMachineEditorWindow.CurrentSelection)
            {
                AppendSelectStateMachineAction(evt);
                
                if(_stateMachine)
                    AppendCreateStateAction(evt);
            }
            
            switch (FsmStateMachineEditorWindow.CurrentSelection)
            {
                case Transition transition:
                    AppendEdgeActions(evt, transition);
                    break;
            }
        }

        private void AppendSelectStateMachineAction(ContextualMenuPopulateEvent evt)
        {
            FsmStateMachine[] instances = EditorUtils.FindAssetsOfType<FsmStateMachine>();
            
            if (instances.Length == 0)
                return;

            FsmStateMachine parentFsm = null;

            foreach (FsmStateMachine stateMachine in instances)
            {
                if (stateMachine.States.FirstOrDefault(s => s.SubStateMachine == _stateMachine) is not
                    { } state) continue;
                
                parentFsm = stateMachine;
                break;
            }

            if (parentFsm)
            {
                evt.menu.AppendAction("Select Parent State Machine",
                    _ => Selection.activeObject = parentFsm);
                
                evt.menu.AppendSeparator();
            }
            
            foreach(FsmStateMachine fsm in instances)
                evt.menu.AppendAction($"Select State Machine/{fsm.name}", 
                    _ => Selection.activeObject = fsm);
        }
        
        private void AppendCreateStateAction(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction($"Create New State",
                _ => CreateState());
        }

        private void AppendEdgeActions(ContextualMenuPopulateEvent evt, Transition transition)
        {
            TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom<TransitionCondition>();
            
            foreach(Type type in types)
                AppendCreateConditionAction(evt, transition, type);
        }

        private void AppendCreateConditionAction(ContextualMenuPopulateEvent evt, Transition transition, Type type)
        {
            if (type.IsGenericType)
                return;
            
            evt.menu.AppendAction($"Add Condition/{type.Name}", _ =>
            {
                if (!transition || !_stateMachine)
                    return;
                
                FsmFactory.CreateCondition(_stateMachine, transition, type);
            });
        }
        
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(endPort =>
            {
                bool portsMatch = endPort.direction != startPort.direction &&
                endPort.node != startPort.node;
                
                if (startPort.node is not FsmStateView from ||
                    endPort.node is not FsmStateView to)
                    return false;
                
                bool doesNotConnectToSelf = from.State != to.State;
                bool isFirstConnection = from.State.Transitions.All(t => t.TargetState != to.State);
                return portsMatch && doesNotConnectToSelf && isFirstConnection;
            }).ToList();
        }
        
        private void CreateState()
        {
            FsmState state = FsmFactory.CreateState(_stateMachine);
            CreateView(state, state.Views[0]);
        }

        private void CreateView(FsmState state, FsmState.ViewData viewData)
        {
            if (string.IsNullOrEmpty(viewData.Guid))
                viewData.Guid = GUID.Generate().ToString();
            
            EditorUtility.SetDirty(state);
            AssetDatabase.SaveAssets();
            
            FsmStateView stateView = new(state, viewData, CreateView, _stateMachine);
            AddElement(stateView);
        }

        private FsmStateView FindView(string guid)
        {
            return GetNodeByGuid(guid) as FsmStateView;
        }

        private void CreateViews(IEnumerable<FsmState> states)
        {
            foreach (FsmState state in states)
            {
                if(state.Views.Count < 1)
                    state.Views.Add(new FsmState.ViewData());
                
                foreach(FsmState.ViewData viewData in state.Views)
                    CreateView(state, viewData);
            }
        }

        private void CreateEdges(FsmState state)
        {
            for (int i = state.Transitions.Count - 1; i > -1; --i)
            {
                Transition transition = state.Transitions[i];
                
                if (!transition)
                    continue;

                if (string.IsNullOrEmpty(transition.GuidFrom))
                {
                    transition.GuidFrom = state.Views[0].Guid;
                    EditorUtility.SetDirty(transition);
                }

                if (string.IsNullOrEmpty(transition.GuidTo))
                {
                    transition.GuidTo = transition.TargetState.Views[0].Guid;
                    EditorUtility.SetDirty(transition);
                }
                
                FsmStateView from = FindView(transition.GuidFrom);

                if (transition.TargetState == from.State)
                {
                    state.Transitions.RemoveAt(i);
                    EditorUtility.SetDirty(state);
                }
                else
                {
                    FsmStateView to = FindView(transition.GuidTo);
                    Edge edge = from.Output.ConnectTo(to.Input);
                    AddElement(edge);
                }
                
                if(EditorUtility.IsDirty(transition) || EditorUtility.IsDirty(state))
                    AssetDatabase.SaveAssets();
            }
        }
    }
}