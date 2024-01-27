using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    [CustomEditor(typeof(FsmState), editorForChildClasses: true)]
    public class FsmStateEditor : Editor
    {
        private string _name;

        private void OnEnable()
        {
            if(target)
                _name = target.name;
        }

        public override void OnInspectorGUI()
        {
            _name = EditorGUILayout.TextField("Name", _name);

            if (GUILayout.Button("Apply Name"))
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(target), _name);
            
            EditorGUILayout.Space(20f);
            SelectStateMachine();
            EditorGUILayout.Space(20f);
            ShowSubFsmMenu();
            EditorGUILayout.Space(10f);
            
            base.OnInspectorGUI();
        }

        private void SelectStateMachine()
        {
            if (target is not FsmState state)
                return;

            if (!GUILayout.Button("Select State Machine"))
                return;
            
            FsmStateMachine[] stateMachines = EditorUtils.FindAssetsOfType<FsmStateMachine>();
            FsmStateMachine stateMachine = stateMachines?.FirstOrDefault(fsm => fsm.States.Contains(state));

            if (!stateMachine)
                return;
            
            FsmStateMachineEditorWindow.ShowWindow();
            Selection.activeObject = stateMachine;
        }
        
        private void ShowSubFsmMenu()
        {
            if (target is not FsmState state)
                return;

            state.StateMachine = EditorGUILayout.ObjectField("Sub State Machine", 
                state.StateMachine, typeof(FsmStateMachine), false) as FsmStateMachine;

            if (!state.StateMachine && GUILayout.Button("Create Sub State Machine"))
                FsmFactory.CreateSubStateMachine(state);

            serializedObject.ApplyModifiedProperties();
        }
    }
}