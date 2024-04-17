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
                {
                    FsmTransition transition = FsmFactory.CreateTransition(StateMachine, from.State, to.State);
                    transition.GuidFrom = from.ViewData.Guid;
                    transition.GuidTo = to.ViewData.Guid;
                }
            });
            
            return graphViewChange;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (!FsmStateMachineEditorWindow.ActiveObject)
            {
                AppendSelectStateMachineAction(evt);
                
                if(StateMachine)
                    AppendCreateStateAction(evt);
            }
            
            switch (FsmStateMachineEditorWindow.ActiveObject)
            {
                case FsmTransition transition:
                    AppendEdgeActions(evt, transition);
                    break;
            }

            if (FsmStateMachineEditorWindow.SelectedStates.Count > 0)
            {
                AppendMergeIntoStateMachineAction(evt, FsmStateMachineEditorWindow.SelectedStates);
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

        private void AppendEdgeActions(ContextualMenuPopulateEvent evt, FsmTransition fsmTransition)
        {
            TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom<FsmCondition>();
            
            foreach(Type type in types)
                AppendCreateConditionAction(evt, fsmTransition, type);
        }

        private void AppendCreateConditionAction(ContextualMenuPopulateEvent evt, FsmTransition fsmTransition, Type type)
        {
            if (type.IsGenericType)
                return;
            
            evt.menu.AppendAction($"Add Condition/{type.Name}", _ =>
            {
                if (!fsmTransition || !StateMachine)
                    return;
                
                FsmFactory.CreateCondition(StateMachine, fsmTransition, type);
            });
        }

        private void AppendMergeIntoStateMachineAction(ContextualMenuPopulateEvent evt, List<FsmState> states)
        {
            evt.menu.AppendAction("Selected States/Merge into State Machine", _ =>
            {
                MergeSelectionIntoStateMachine(states);
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
                FsmTransition fsmTransition = state.Transitions[i];
                
                if (!fsmTransition)
                    continue;

                if (string.IsNullOrEmpty(fsmTransition.GuidFrom))
                {
                    fsmTransition.GuidFrom = state.Views[0].Guid;
                    EditorUtility.SetDirty(fsmTransition);
                }

                if (string.IsNullOrEmpty(fsmTransition.GuidTo))
                {
                    fsmTransition.GuidTo = fsmTransition.TargetState.Views[0].Guid;
                    EditorUtility.SetDirty(fsmTransition);
                }
                
                FsmStateView from = FindView(fsmTransition.GuidFrom);

                if (fsmTransition.TargetState == from.State)
                {
                    state.Transitions.RemoveAt(i);
                    EditorUtility.SetDirty(state);
                }
                else
                {
                    FsmStateView to = FindView(fsmTransition.GuidTo);
                    Edge edge = from.Output.ConnectTo(to.Input);
                    AddElement(edge);
                }
                
                if(EditorUtility.IsDirty(fsmTransition) || EditorUtility.IsDirty(state))
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
            
            foreach (FsmState state in states)
            {
                AssetDatabase.RemoveObjectFromAsset(state);
                AssetDatabase.AddObjectToAsset(state, childFsm);
                
                parentFsm.RemoveState(state);
                childFsm.AddState(state);
            }
            
            List<FsmState> incoming = GetAllIncomingStates(states);
            
            foreach (FsmState state in incoming)
            {
                FsmTransition toParent =
                    FsmFactory.CreateTransition(StateMachine, state, parentState);                
                
                for (int i = state.Transitions.Count-1; i > -1; i--)
                {
                    FsmTransition transition = state.Transitions[i];
                    
                    if (!states.Contains(transition.TargetState))
                        continue;

                    foreach (FsmCondition condition in transition.Conditions)
                    {
                        if (!toParent.Conditions.Contains(condition))
                            toParent.Conditions.Add(condition);
                    }

                    FsmTransition toState = FsmFactory.CreateTransition(childFsm, childFsm.EntryState, transition.TargetState);
                    
                    foreach(FsmCondition condition in transition.Conditions)
                        toState.Conditions.Add(condition);
                    
                    FsmFactory.DeleteTransition(state, transition.TargetState);
                }
            }
            
            StateMachine.RecalculateNames();
            AssetDatabase.SaveAssets();
            
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