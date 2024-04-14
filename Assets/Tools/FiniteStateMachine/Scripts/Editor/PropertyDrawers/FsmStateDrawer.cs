using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    [CustomPropertyDrawer(typeof(FsmState))]
    public class FsmStateDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            float buttonWidth = Application.isPlaying ? 40f : 0f;
            const float padding = 2f;

            Rect assetPos = position;
            assetPos.width -= buttonWidth + padding;

            Rect buttonPos = position;
            buttonPos.x += position.width - buttonWidth + padding;
            buttonPos.width = buttonWidth;

            //EditorGUI.PropertyField(assetPos, property, new GUIContent());
            DrawStatePropertyField(assetPos, property, label);
            
            if (Application.isPlaying && property.objectReferenceValue is FsmState state)
            {
                GUIContent settingsContent = state.IsActive
                        ? new GUIContent("Exit", "State is currently active. Click to exit.")
                        : new GUIContent("Enter", "State is currently inactive. Click to enter.");
                
                Color prevBackgroundColor = GUI.backgroundColor;
                GUI.backgroundColor = state.IsActive ? Color.red : Color.green;
                
                if (GUI.Button(buttonPos, settingsContent, EditorStyles.miniButtonRight) && property.objectReferenceValue)
                {
                    if (state.IsActive)
                        state.Exit();
                    else
                        EditorUtils.ForceEnterState(state);
                }
            }
            
            EditorGUI.EndProperty();
        }

        private static void DrawStatePropertyField(Rect position, SerializedProperty property, GUIContent label)
        {
            const float buttonWidth = 20f;
            Rect propertyPos = position;
            propertyPos.width -= buttonWidth;

            Rect buttonPos = position;
            buttonPos.x += propertyPos.width;
            buttonPos.width = buttonWidth;

            GUIStyle style = EditorStyles.objectFieldThumb;
            EditorGUI.ObjectField(propertyPos, property, label);
            
            if (!GUI.Button(buttonPos, string.Empty, "objectFieldButton"))
                return;
            
            StateSelectionDropDown drp = new (new AdvancedDropdownState(), state =>
            {
                property.objectReferenceValue = state;
                property.serializedObject.ApplyModifiedProperties();
            });
            drp.Show(position);
        }
    }
}