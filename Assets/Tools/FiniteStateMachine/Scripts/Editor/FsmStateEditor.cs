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

            if (target is FsmState state)
            {
                EditorGUILayout.Space(20f);
                SelectStateMachine(state);
                EditorGUILayout.Space(20f);
                ShowSubFsmMenu(state);
                EditorGUILayout.Space(10f);
            }
            base.OnInspectorGUI();
        }

        private static void SelectStateMachine(FsmState state)
        {
            if (!GUILayout.Button("Select State Machine"))
                return;
            
            FsmStateMachine[] stateMachines = EditorUtils.FindAssetsOfType<FsmStateMachine>();
            FsmStateMachine stateMachine = stateMachines?.FirstOrDefault(fsm => fsm.States.Contains(state));

            if (!stateMachine)
                return;
            
            FsmStateMachineEditorWindow.ShowWindow();
            Selection.activeObject = stateMachine;
        }
        
        private void ShowSubFsmMenu(FsmState state)
        {
            state.StateMachine = EditorGUILayout.ObjectField("Sub State Machine", 
                state.StateMachine, typeof(FsmStateMachine), false) as FsmStateMachine;

            if (!state.StateMachine && GUILayout.Button("Create Sub State Machine"))
                FsmFactory.CreateSubStateMachine(state);

            serializedObject.ApplyModifiedProperties();
        }
    }
}