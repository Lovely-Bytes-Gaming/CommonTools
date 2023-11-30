#if UNITY_EDITOR
using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    public partial class Transition
    {
        [SerializeField, HideInInspector] 
        internal string GuidFrom, GuidTo;
    }
}
#endif