using UnityEditor;
using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    [CustomPropertyDrawer(typeof(FsmState))]
    public class FsmStatePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            float buttonWidth = Application.isPlaying ? 40f : 0f;
            const float padding = 2f;

            Rect assetPos = position;
            assetPos.width -= buttonWidth + padding;

            Rect buildSettingsPos = position;
            buildSettingsPos.x += position.width - buttonWidth + padding;
            buildSettingsPos.width = buttonWidth;

            EditorGUI.PropertyField(assetPos, property, new GUIContent());

            if (Application.isPlaying && property.objectReferenceValue is FsmState state)
            {
                GUIContent settingsContent = state.IsActive
                        ? new GUIContent("Exit", "State is currently active. Click to exit.")
                        : new GUIContent("Enter", "State is currently inactive. Click to enter.");
                
                Color prevBackgroundColor = GUI.backgroundColor;
                GUI.backgroundColor = state.IsActive ? Color.red : Color.green;
                
                if (GUI.Button(buildSettingsPos, settingsContent, EditorStyles.miniButtonRight) && property.objectReferenceValue)
                {
                    if (state.IsActive)
                        state.Exit();
                    else
                        EditorUtils.ForceEnterState(state);
                }
            }
            
            EditorGUI.EndProperty();
        }
    }
}