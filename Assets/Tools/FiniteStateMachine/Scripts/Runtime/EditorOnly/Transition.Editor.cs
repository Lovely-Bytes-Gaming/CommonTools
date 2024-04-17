#if UNITY_EDITOR
using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    public partial class FsmTransition
    {
        [SerializeField, HideInInspector] 
        internal string GuidFrom, GuidTo;
    }
}
#endif