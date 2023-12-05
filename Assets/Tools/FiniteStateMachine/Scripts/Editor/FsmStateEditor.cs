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
            
            EditorGUILayout.Space(10f);
            ShowSubFsmMenu();
            EditorGUILayout.Space(10f);
            base.OnInspectorGUI();
        }

        private void ShowSubFsmMenu()
        {
            if (target is not FsmState state)
                return;

            state.SubStateMachine = EditorGUILayout.ObjectField("Sub State Machine", 
                state.SubStateMachine, typeof(FsmStateMachine), false) as FsmStateMachine;

            if (!state.SubStateMachine && GUILayout.Button("Create Sub State Machine"))
                FsmFactory.CreateSubStateMachine(state);

            serializedObject.ApplyModifiedProperties();
        }
    }
}