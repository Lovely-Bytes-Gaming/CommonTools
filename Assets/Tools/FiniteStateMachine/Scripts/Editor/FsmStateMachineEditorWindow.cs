using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    public class FsmStateMachineEditorWindow : EditorWindow
    {
        private FsmStateMachineView _stateMachineView;
        private InspectorView _inspectorView;

        public static Object CurrentSelection { get; private set; }
        
        [MenuItem("Window/LovelyBytes/State Machine Editor")]
        public static void ShowWindow()
        {
            FsmStateMachineEditorWindow wnd = GetWindow<FsmStateMachineEditorWindow>();
            wnd.titleContent = new GUIContent("FsmStateMachineEditor");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            string rootDirectory = UIBuilderUtils.GetParentDirectory(nameof(FsmStateMachineEditorWindow));
            
            VisualTreeAsset visualTree = AssetDatabase
                .LoadAssetAtPath<VisualTreeAsset>($"{rootDirectory}/FsmStateMachineEditor.uxml");
            
            visualTree.CloneTree(root);
            
            StyleSheet styleSheet = AssetDatabase
                .LoadAssetAtPath<StyleSheet>($"{rootDirectory}/FsmStateMachineEditor.uss");
            
            root.styleSheets.Add(styleSheet);

            _stateMachineView = root.Q<FsmStateMachineView>();
            _inspectorView = root.Q<InspectorView>();
            
            OnSelectionChange();
        }

        private void OnSelectionChange()
        {
            FsmStateMachine stateMachine = Selection.activeObject as FsmStateMachine;

            if (!stateMachine)
                return;

            stateMachine.DoInitializeStatesForEditor();
            _stateMachineView.PopulateView(stateMachine);
            
            UQueryState<Node> nodes = _stateMachineView.nodes;

            foreach (Node node in nodes)
            {
                if (node is not FsmStateView view)
                    continue;
                
                if(view.State.IsActive)
                    view.Select(_stateMachineView, additive: true);
            }
        }

        private void OnInspectorUpdate()
        {
            UQueryState<Edge> edges = _stateMachineView.edges;
            UQueryState<Node> nodes = _stateMachineView.nodes;

            foreach (Node node in nodes)
            {
                if (node is FsmStateView view)
                    UpdateStateView(view);
            }
            
            Object lastSelected = CurrentSelection;
            UpdateSelection(nodes, edges);
            
            if(!lastSelected || lastSelected != CurrentSelection)
                _inspectorView.UpdateSelection(CurrentSelection);
        }

        private static void UpdateStateView(FsmStateView view)
        {
            FsmState state = view.State;
            
            if (state.name != view.title)
                view.title = state.name;

            Color deselectedColor = state.SubStateMachine 
                ? new Color(1f, 0.0f, 0.75f, 0.25f) 
                : new Color();
            
            view.style.backgroundColor = state.IsActive
                ? Color.green
                : deselectedColor;
        }
        
        private void UpdateSelection(in UQueryState<Node> nodes, in UQueryState<Edge> edges)
        {
            foreach (Node node in nodes)
            {
                if (node is not FsmStateView view || !view.IsSelected(_stateMachineView))
                    continue;
                
                CurrentSelection = view.State;
                return;
            }
            
            foreach (Edge edge in edges.Where(edge => edge.IsSelected(_stateMachineView)))
            {
                if (edge.output.node is not FsmStateView from ||
                    edge.input.node is not FsmStateView to) 
                    continue;

                foreach (Transition transition in from.State.Transitions.Where(transition => 
                             transition && transition.TargetState == to.State))
                {
                    CurrentSelection = transition;
                    return;
                }
            }

            CurrentSelection = null;
        }
    }
}

