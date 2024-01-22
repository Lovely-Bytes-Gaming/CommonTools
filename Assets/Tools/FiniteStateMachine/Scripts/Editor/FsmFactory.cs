
using System;
using UnityEditor;
using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    internal static class FsmFactory 
    {
        private const string _stateDir = "States";
        private const string _transitionDir = "Transitions";
        private const string _conditionDir = "Conditions";
        private const string _behaviourDir = "Behaviour";
        
        public static FsmState CreateState(FsmStateMachine parentFsm)
        {
            FsmState state = ScriptableObject.CreateInstance<FsmState>();

            if (!state)
                return null;
            
            state.name = $"{parentFsm.name}_State_{parentFsm.States.Count}";

            state.Views.Add(new FsmState.ViewData
            {
                Guid = GUID.Generate().ToString(),
                CanvasPosition = Vector2.zero
            });

            EditorUtils.SaveAsset(state, parentFsm, _stateDir);
            parentFsm.AddState(state);
            
            EditorUtility.SetDirty(state);
            EditorUtility.SetDirty(parentFsm);
            AssetDatabase.SaveAssets();

            return state;
        }
        
        public static void DeleteViewData(FsmStateMachine parentFsm, FsmState state, FsmState.ViewData view)
        {
            state.Views.Remove(view);

            if (state.Views.Count < 1)
            {
                parentFsm.RemoveState(state);
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(state));
                EditorUtility.SetDirty(parentFsm);
            }
            else
            {
                EditorUtility.SetDirty(state);
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

            transition.name = $"{from.State.name}_GOTO_{to.State.name}";
            transition.TargetState = to.State;
            transition.GuidFrom = from.ViewData.Guid;
            transition.GuidTo = to.ViewData.Guid;
            EditorUtils.SaveAsset(transition, parentFsm, _transitionDir);
            
            from.State.Transitions.Add(transition);
            EditorUtility.SetDirty(from.State);
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
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(transition));
            
            EditorUtility.SetDirty(from);
            AssetDatabase.SaveAssets();
        }
        
        public static TransitionCondition CreateCondition(FsmStateMachine parentFsm, Transition transition, Type conditionType)
        {
            TransitionCondition condition = ScriptableObject.CreateInstance(conditionType) 
                as TransitionCondition;
            
            if (!condition)
                return null;
            
            condition.name = $"{transition.name}_{conditionType.Name}";
            EditorUtils.SaveAsset(condition, parentFsm, _conditionDir);
            
            transition.Conditions.Add(condition);
            EditorUtility.SetDirty(transition);
            AssetDatabase.SaveAssets();

            return condition;
        }
        
        public static FsmBehaviour CreateBehaviour(FsmStateMachine parentFsm, FsmState state, Type behaviourType)
        {
            FsmBehaviour behaviour = ScriptableObject.CreateInstance(behaviourType) as FsmBehaviour;

            if (!behaviour)
                return null;

            behaviour.name = $"{state.name}_{behaviourType.Name}";
            EditorUtils.SaveAsset(behaviour, parentFsm, _behaviourDir);
            
            state.Behaviours.Add(behaviour);
            EditorUtility.SetDirty(state);
            AssetDatabase.SaveAssets();

            return behaviour;
        }

        public static FsmStateMachine CreateSubStateMachine(FsmState parentState)
        {
            FsmStateMachine fsm = ScriptableObject.CreateInstance<FsmStateMachine>();
            fsm.name = $"{parentState.name}FSM";
            EditorUtils.SaveAsset(fsm, parentState, fsm.name);
            
            parentState.StateMachine = fsm;
            EditorUtility.SetDirty(parentState);
            AssetDatabase.SaveAssets();

            return fsm;
        }
    }
}