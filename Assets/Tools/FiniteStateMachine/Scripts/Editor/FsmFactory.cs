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
            
            FsmStateMachineEditorWindow.ShowWindow();
            Selection.activeObject = stateMachine;
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
        
        public static Transition CreateTransition(FsmStateMachine parentFsm, FsmStateView from, FsmStateView to)
        {
            foreach (Transition t in from.State.Transitions)
            {
                if (t.TargetState == to.State)
                    return null;
            }
            
            Transition transition = ScriptableObject.CreateInstance<Transition>();

            transition.TargetState = to.State;
            transition.GuidFrom = from.ViewData.Guid;
            transition.GuidTo = to.ViewData.Guid;
            from.State.Transitions.Add(transition);
            parentFsm.RecalculateNames();
            
            AssetDatabase.AddObjectToAsset(transition, from.State);
            AssetDatabase.SaveAssets();
            
            return transition;
        }
        
        public static void DeleteTransition(FsmState from, FsmState to)
        {
            Transition transition = default;
            
            foreach (Transition t in from.Transitions)
            {
                if (!t || t.TargetState != to)
                    continue;

                transition = t;
                break;
            }

            if (!transition)
                return;
            
            from.Transitions.Remove(transition);
            AssetDatabase.RemoveObjectFromAsset(transition);
            AssetDatabase.SaveAssets();
        }
        
        public static TransitionCondition CreateCondition(FsmStateMachine parentFsm, Transition transition, Type conditionType)
        {
            TransitionCondition condition = ScriptableObject.CreateInstance(conditionType) 
                as TransitionCondition;
            
            if (!condition)
                return null;
            
            transition.Conditions.Add(condition);
            parentFsm.RecalculateNames();
            
            AssetDatabase.AddObjectToAsset(condition, transition);
            AssetDatabase.SaveAssets();

            return condition;
        }

        public static void DeleteCondition(Transition transition, TransitionCondition condition)
        {
            transition.Conditions.Remove(condition);
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