using UnityEditor;
using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    [CustomEditor(typeof(FsmStateMachine))]
    public class FsmStateMachineEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Open Graph View"))
            {
                FsmStateMachineEditorWindow.ShowWindow();
            }
        }
    }
}