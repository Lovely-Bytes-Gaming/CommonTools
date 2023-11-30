using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    [CustomEditor(typeof(TimerCondition))]
    public class TimerConditionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (target is not TimerCondition timerCondition)
                return;

            EditorGUILayout.Space(20f);

            FieldInfo fieldInfo = typeof(TimerCondition).GetField("_time",
                BindingFlags.Instance | BindingFlags.NonPublic);

            if(fieldInfo?.GetValue(timerCondition) is float time)
                EditorGUILayout.FloatField("Time", time);
            
            if (!Application.isPlaying)
                return;
        }
    }
}