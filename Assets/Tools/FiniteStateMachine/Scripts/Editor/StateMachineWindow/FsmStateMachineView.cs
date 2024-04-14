using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace LovelyBytes.CommonTools.FiniteStateMachine 
{
    public class FsmStateMachineView : GraphView 
    {
        public new class UxmlFactory : UxmlFactory<FsmStateMachineView, UxmlTraits> { }
        public FsmStateMachine StateMachine { get; private set; }

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
            StateMachine = stateMachine;

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;

            CreateViews(StateMachine.States);

            foreach (FsmState state in StateMachine.States)
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
                        FsmFactory.DeleteViewData(StateMachine, stateView.State, stateView.ViewData);
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
                    FsmFactory.CreateTransition(StateMachine, from, to);
            });
            
            return graphViewChange;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (!FsmStateMachineEditorWindow.CurrentSelection)
            {
                AppendSelectStateMachineAction(evt);
                
                if(StateMachine)
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

            FsmStateMachine parentFsm = GetParentStateMachine(StateMachine, instances);

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
                if (!transition || !StateMachine)
                    return;
                
                FsmFactory.CreateCondition(StateMachine, transition, type);
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
            FsmState state = FsmFactory.CreateState(StateMachine);
            CreateView(state, state.Views[0]);
        }

        private void CreateView(FsmState state, FsmState.ViewData viewData)
        {
            if (string.IsNullOrEmpty(viewData.Guid))
                viewData.Guid = GUID.Generate().ToString();
            
            EditorUtility.SetDirty(state);
            AssetDatabase.SaveAssets();
            
            FsmStateView stateView = new(state, viewData, CreateView, StateMachine);
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

        private static FsmStateMachine GetParentStateMachine(FsmStateMachine stateMachine,
            IEnumerable<FsmStateMachine> availableStateMachines)
        {
            if (!stateMachine)
                return null;

            return availableStateMachines.FirstOrDefault(fsm =>
                fsm.States.Any(state =>
                    state.SubStateMachine == stateMachine));
        }

        private void MergeSelectionIntoStateMachine(List<FsmState> states)
        {
            FsmStateMachine parentFsm = StateMachine;
            FsmState parentState = FsmFactory.CreateState(parentFsm);
            FsmStateMachine childFsm = FsmFactory.CreateSubStateMachine(parentState);

            List<FsmState> incoming = GetAllIncomingStates(states);
            
            

            // --- Handling incoming transitions ---
            // 1. Find all states that can transition into our selected set of states.
            // 2. For each incoming transition, create a new transition that links to the new parent state.
            //  2.a If this results in a state having multiple transitions to the same state,
            //      Merge them into a single transition that contains the disjunction of all their conditions
            // 3. Create a transition from the new state machine's entry state to each of the states they targeted before.

            // --- Handling outgoing transitions
            // 1. Find all states within our set that have one or more transitions to outside states.
            // 2. Link all those transitions to the new state machine's exit state.
            //  2.a If this results in a state having multiple transitions to the same state,
            //      Merge them into a single transition that contains the disjunction of all their conditions
            // 3. Create a transition from the new parent state to each of the states they targeted before.
        }

        private List<FsmState> GetAllIncomingStates(ICollection<FsmState> states)
        {
            return StateMachine.States
                .Except(states)
                .Where(state => state.Transitions.Any(trans => states.Contains(trans.TargetState)))
                .ToList();
        }

        private List<FsmState> GetAllOutgoingStates(ICollection<FsmState> states)
        {
            return states
                .Where(state => state.Transitions.Any(trans => !states.Contains(trans.TargetState)))
                .ToList();
        }
    } 
}