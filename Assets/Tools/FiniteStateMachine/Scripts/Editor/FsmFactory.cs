using System;
using UnityEditor;
using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    internal static class FsmFactory
    {
        [MenuItem("Assets/Create/LovelyBytes/CommonTools/StateMachine")]
        public static void CreateStateMachine() => CreateStateMachine("New State Machine");
        
        public static FsmStateMachine CreateStateMachine(string name)
        {
            string path = !Selection.activeObject 
                ? "Assets" : AssetDatabase.GetAssetPath(Selection.activeObject);

            if (!AssetDatabase.IsValidFolder(path))
            {
                int nameIndex = path.LastIndexOf('/');
                path = path[..nameIndex];
            }

            FsmStateMachine stateMachine = ScriptableObject.CreateInstance<FsmStateMachine>();
            stateMachine.name = name;
            EditorUtils.CreateAsset(stateMachine, path);
            
            FsmState entry = CreateState(stateMachine, "Entry");
            FsmState exit = CreateState(stateMachine, "Exit");

            entry.Views[0].CanvasPosition = new Vector2(-100, 0);
            exit.Views[0].CanvasPosition = new Vector2(100, 0);
            
            stateMachine.EntryState = entry;
            stateMachine.ExitState = exit;
            AssetDatabase.SaveAssets();
            
            return stateMachine;
        }
        
        public static FsmState CreateState(FsmStateMachine parentFsm, string name = null)
        {
            FsmState state = ScriptableObject.CreateInstance<FsmState>();

            if (!state)
                return null;

            state.name = string.IsNullOrEmpty(name)
                ? $"{parentFsm.name}_State_{parentFsm.States.Count}"
                : name;

            state.Views.Add(new FsmState.ViewData
            {
                Guid = GUID.Generate().ToString(),
                CanvasPosition = Vector2.zero
            });
            parentFsm.AddState(state);
            
            AssetDatabase.AddObjectToAsset(state, parentFsm);
            AssetDatabase.SaveAssets();

            return state;
        }
        
        public static void DeleteViewData(FsmStateMachine parentFsm, FsmState state, FsmState.ViewData view)
        {
            state.Views.Remove(view);

            if (state.Views.Count < 1)
            {
                parentFsm.RemoveState(state);
                AssetDatabase.RemoveObjectFromAsset(state);
            }
            AssetDatabase.SaveAssets();
        }
        
        public static FsmTransition CreateTransition(FsmStateMachine parentFsm, 
            FsmState from, FsmState to)
        {
            foreach (FsmTransition t in from.Transitions)
            {
                if (t.TargetState == to)
                    return null;
            }
            
            FsmTransition fsmTransition = ScriptableObject.CreateInstance<FsmTransition>();

            fsmTransition.TargetState = to;
            from.Transitions.Add(fsmTransition);
            parentFsm.RecalculateNames();
            
            AssetDatabase.AddObjectToAsset(fsmTransition, from);
            AssetDatabase.SaveAssets();
            
            return fsmTransition;
        }
        
        public static void DeleteTransition(FsmState from, FsmState to)
        {
            FsmTransition fsmTransition = default;
            
            foreach (FsmTransition t in from.Transitions)
            {
                if (!t || t.TargetState != to)
                    continue;

                fsmTransition = t;
                break;
            }

            if (!fsmTransition)
                return;
            
            from.Transitions.Remove(fsmTransition);
            AssetDatabase.RemoveObjectFromAsset(fsmTransition);
            AssetDatabase.SaveAssets();
        }
        
        public static FsmCondition CreateCondition(FsmStateMachine parentFsm, FsmTransition fsmTransition, Type conditionType)
        {
            FsmCondition condition = ScriptableObject.CreateInstance(conditionType) 
                as FsmCondition;
            
            if (!condition)
                return null;
            
            fsmTransition.Conditions.Add(condition);
            parentFsm.RecalculateNames();
            
            AssetDatabase.AddObjectToAsset(condition, fsmTransition);
            AssetDatabase.SaveAssets();

            return condition;
        }

        public static void DeleteCondition(FsmTransition fsmTransition, FsmCondition condition)
        {
            fsmTransition.Conditions.Remove(condition);
            AssetDatabase.RemoveObjectFromAsset(condition);
            AssetDatabase.SaveAssets();
        }
        
        public static FsmBehaviour CreateBehaviour(FsmState state, Type behaviourType)
        {
            FsmBehaviour behaviour = ScriptableObject.CreateInstance(behaviourType) as FsmBehaviour;

            if (!behaviour)
                return null;

            behaviour.name = $"{state.name}: {behaviourType.Name}";
            state.Behaviours.Add(behaviour);
            
            AssetDatabase.AddObjectToAsset(behaviour, state);
            AssetDatabase.SaveAssets();

            return behaviour;
        }

        public static void DeleteBehaviour(FsmState state, FsmBehaviour behaviour)
        {
            state.Behaviours.Remove(behaviour);
            AssetDatabase.RemoveObjectFromAsset(behaviour);
            AssetDatabase.SaveAssets();
        }

        public static FsmStateMachine CreateSubStateMachine(FsmState parentState)
        {
            FsmStateMachine fsm = CreateStateMachine($"{parentState.name}FSM");
            parentState.SubStateMachine = fsm;
            AssetDatabase.SaveAssets();
            return fsm;
        }
    }
}